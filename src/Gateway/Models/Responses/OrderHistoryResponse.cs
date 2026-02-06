using Gateway.Models.Enums;
using Gateway.Models.Payloads;

namespace Gateway.Models.Responses;

public record OrderHistoryResponse(
    DateTime CreatedAt,
    OrderHistoryItemType HistoryItemType,
    HistoryPayloadBase? Payload);