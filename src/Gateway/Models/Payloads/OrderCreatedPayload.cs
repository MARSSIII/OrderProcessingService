namespace Gateway.Models.Payloads;

public record OrderCreatedPayload(string CreatedBy) : HistoryPayloadBase;