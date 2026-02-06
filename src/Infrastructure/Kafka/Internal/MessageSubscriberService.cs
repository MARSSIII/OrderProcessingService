using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading.Channels;
using Infrastructure.Kafka.Abstractions;
using Infrastructure.Kafka.Options;

namespace Infrastructure.Kafka.Internal;

internal sealed class MessageSubscriberService<TMessage> : BackgroundService
    where TMessage : class, new()
{
    private readonly SubscriptionOptions _subscriptionOptions;

    private readonly MessagingOptions _messagingOptions;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IMessageSerializer _serializer;

    private readonly Channel<ConsumeResult<string, byte[]>> _channel;

    private IConsumer<string, byte[]>? _consumer;

    public MessageSubscriberService(
        string subscriptionName,
        IOptions<MessagingOptions> options,
        IServiceScopeFactory scopeFactory,
        IMessageSerializer serializer)
    {
        _messagingOptions = options.Value;
        _subscriptionOptions = _messagingOptions.Subscriptions[subscriptionName];
        _scopeFactory = scopeFactory;
        _serializer = serializer;

        _channel = Channel.CreateBounded<ConsumeResult<string, byte[]>>(
            new BoundedChannelOptions(_subscriptionOptions.ChannelCapacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true,
            });
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        _consumer = BuildConsumer();
        _consumer.Subscribe(_subscriptionOptions.Topic);

        Task readTask = ReadFromKafkaAsync(stoppingToken);
        Task processTask = ProcessMessagesInBatchesAsync(stoppingToken);

        await Task.WhenAll(readTask, processTask);
    }

    private async Task ReadFromKafkaAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<string, byte[]>? result = _consumer?.Consume(
                        TimeSpan.FromMilliseconds(_subscriptionOptions.PollTimeoutMs));

                    if (result is not null && result.Message is not null)
                    {
                        await _channel.Writer.WriteAsync(result, ct);
                    }
                }
                catch (ConsumeException)
                {
                    await Task.Delay(500, ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _channel.Writer.Complete();
        }
    }

    private async Task ProcessMessagesInBatchesAsync(CancellationToken ct)
    {
        var batch = new List<ConsumeResult<string, byte[]>>(_subscriptionOptions.MaxBatchSize);
        var batchDelayTimeout = TimeSpan.FromMilliseconds(_subscriptionOptions.MaxBatchDelayMs);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            while (!ct.IsCancellationRequested || await _channel.Reader.WaitToReadAsync(ct))
            {
                stopwatch.Restart();

                while (batch.Count < _subscriptionOptions.MaxBatchSize)
                {
                    TimeSpan remainingTime = batchDelayTimeout - stopwatch.Elapsed;

                    if (remainingTime <= TimeSpan.Zero)
                    {
                        break;
                    }

                    if (_channel.Reader.TryRead(out ConsumeResult<string, byte[]>? result))
                    {
                        batch.Add(result);
                    }
                    else if (batch.Count == 0)
                    {
                        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        timeoutCts.CancelAfter(remainingTime);

                        try
                        {
                            await _channel.Reader.WaitToReadAsync(timeoutCts.Token);
                        }
                        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (!await WaitToReadWithTimeoutAsync(_channel.Reader, remainingTime, ct))
                        {
                            break;
                        }
                    }
                }

                if (batch.Count > 0)
                {
                    await ProcessBatchAsync(batch, ct);
                    batch.Clear();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }

        if (batch.Count > 0)
        {
            await ProcessBatchAsync(batch, CancellationToken.None);
        }
    }

    private async Task<bool> WaitToReadWithTimeoutAsync(
        ChannelReader<ConsumeResult<string, byte[]>> reader,
        TimeSpan timeout,
        CancellationToken ct)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await reader.WaitToReadAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return false;
        }
    }

    private async Task ProcessBatchAsync(
        List<ConsumeResult<string, byte[]>> batch,
        CancellationToken ct)
    {
        var envelopes = batch
            .Select(CreateEnvelope)
            .ToList();

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        IMessageHandler<TMessage> handler = scope.ServiceProvider
            .GetRequiredService<IMessageHandler<TMessage>>();

        await handler.HandleAsync(envelopes, ct);

        if (_consumer is not null && batch.Count > 0)
        {
            ConsumeResult<string, byte[]> lastResult = batch[^1];
            _consumer.Commit(lastResult);
        }
    }

    private MessageEnvelope<TMessage> CreateEnvelope(ConsumeResult<string, byte[]> result)
    {
        TMessage payload = _serializer.Deserialize<TMessage>(result.Message.Value);

        return new MessageEnvelope<TMessage>
        {
            Payload = payload,
            Topic = result.Topic,
            Key = result.Message.Key ?? string.Empty,
            Partition = result.Partition.Value,
            Offset = result.Offset.Value,
            Timestamp = result.Message.Timestamp.UtcDateTime,
        };
    }

    private IConsumer<string, byte[]> BuildConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _messagingOptions.BootstrapServers,
            GroupId = _subscriptionOptions.GroupId,
            ClientId = _subscriptionOptions.ClientId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        return new ConsumerBuilder<string, byte[]>(config).Build();
    }
}