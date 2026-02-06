using Application.Contracts.Orders.DTOs.Enums;

namespace Application.Contracts.Orders.DTOs.Payloads;

public record StatusChangedPayloadDetails(OrderStateDetails OldState, OrderStateDetails NewState) : PayloadBaseDetails;
