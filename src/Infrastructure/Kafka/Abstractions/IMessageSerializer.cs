namespace Infrastructure.Kafka.Abstractions;

public interface IMessageSerializer
{
    byte[] Serialize<T>(T message) where T : class;

    T Deserialize<T>(ReadOnlySpan<byte> data) where T : class, new();
}