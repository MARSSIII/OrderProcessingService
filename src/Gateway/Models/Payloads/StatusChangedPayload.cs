using Gateway.Models.Enums;

namespace Gateway.Models.Payloads;

public record StatusChangedPayload(
    OrderState OldState,
    OrderState NewState) : HistoryPayloadBase;