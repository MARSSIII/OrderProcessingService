using Npgsql;

namespace Infrastructure.Postgres.Mapping.Ordinals;

public readonly struct OrderOrdinals
{
    public int Id { get; }

    public int State { get; }

    public int CreatedAt { get; }

    public int CreatedBy { get; }

    public OrderOrdinals(NpgsqlDataReader reader)
    {
        Id = reader.GetOrdinal("order_id");
        State = reader.GetOrdinal("order_state");
        CreatedAt = reader.GetOrdinal("order_created_at");
        CreatedBy = reader.GetOrdinal("order_created_by");
    }
}