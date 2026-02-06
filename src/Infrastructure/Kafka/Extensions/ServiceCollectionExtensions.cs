using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Application.Abstractions.Messaging;
using Infrastructure.Kafka.Abstractions;
using Infrastructure.Kafka.Internal;
using Infrastructure.Kafka.Options;
using Infrastructure.Kafka.Publishers;

namespace Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddOptions<MessagingOptions>().BindConfiguration("kafka");

        services.AddSingleton<IMessageSerializer, ProtobufMessageSerializer>();
        services.AddSingleton<IMessageBroker, KafkaMessageBroker>();
        services.AddSingleton<IOrderEventPublisher, OrderEventPublisher>();

        return services;
    }

    public static IServiceCollection AddMessageSubscriber<TMessage, THandler>(
        this IServiceCollection services,
        string subscriptionName
    )
        where TMessage : class, new()
        where THandler : class, IMessageHandler<TMessage>
    {
        services.AddScoped<IMessageHandler<TMessage>, THandler>();

        services.AddSingleton<IHostedService>(sp =>
        {
            IOptions<MessagingOptions> options = sp.GetRequiredService<
                IOptions<MessagingOptions>
            >();
            IServiceScopeFactory scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            IMessageSerializer serializer = sp.GetRequiredService<IMessageSerializer>();

            return new MessageSubscriberService<TMessage>(
                subscriptionName,
                options,
                scopeFactory,
                serializer
            );
        });

        return services;
    }
}

