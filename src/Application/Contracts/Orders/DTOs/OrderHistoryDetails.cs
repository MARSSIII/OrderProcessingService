using Application.Contracts.Orders.DTOs.Enums;
using Application.Contracts.Orders.DTOs.Payloads;

namespace Application.Contracts.Orders.DTOs;

public record OrderHistoryDetails(
    DateTime CreatedAt,
    OrderHistoryItemDetails HistoryItem,
    PayloadBaseDetails Payload);
