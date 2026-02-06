using Domain.Orders.Enums;

namespace Domain.Orders.Payloads;

public record StatusChangedPayload(OrderState OldState, OrderState NewState) : PayloadBase;
