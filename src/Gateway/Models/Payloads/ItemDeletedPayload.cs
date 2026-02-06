namespace Gateway.Models.Payloads;

public record ItemDeletedPayload(long ProductId) : HistoryPayloadBase;