using Google.Protobuf;
using Infrastructure.Kafka.Abstractions;

namespace Infrastructure.Kafka.Internal;

internal sealed class ProtobufMessageSerializer : IMessageSerializer
{
    public byte[] Serialize<T>(T message) where T : class
    {
        if (message is not IMessage proto)
            throw new InvalidOperationException($"{typeof(T).Name} is not a protobuf message");

        return proto.ToByteArray();
    }

    public T Deserialize<T>(ReadOnlySpan<byte> data) where T : class, new()
    {
        var message = new T();
        if (message is IMessage proto)
        {
            proto.MergeFrom(data);
        }

        return message;
    }
}