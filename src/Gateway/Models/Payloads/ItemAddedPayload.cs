namespace Gateway.Models.Payloads;

public record ItemAddedPayload(
    long ProductId,
    int Quantity) : HistoryPayloadBase;