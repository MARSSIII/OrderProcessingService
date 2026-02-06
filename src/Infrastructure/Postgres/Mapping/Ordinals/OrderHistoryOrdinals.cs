using Npgsql;

namespace Infrastructure.Postgres.Mapping.Ordinals;

public readonly struct OrderHistoryOrdinals
{
    public int Id { get; }

    public int OrderId { get; }

    public int CreatedAt { get; }

    public int HistoryItem { get; }

    public int Payload { get; }

    public OrderHistoryOrdinals(NpgsqlDataReader reader)
    {
        Id = reader.GetOrdinal("order_history_item_id");
        OrderId = reader.GetOrdinal("order_id");
        CreatedAt = reader.GetOrdinal("order_history_item_created_at");
        HistoryItem = reader.GetOrdinal("order_history_item_kind");
        Payload = reader.GetOrdinal("order_history_item_payload");
    }
}