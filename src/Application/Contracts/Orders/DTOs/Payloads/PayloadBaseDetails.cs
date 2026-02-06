using System.Text.Json.Serialization;

namespace Application.Contracts.Orders.DTOs.Payloads;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(OrderCreatedByPayloadDetails), "OrderCreatedPayload")]
[JsonDerivedType(typeof(StatusChangedPayloadDetails), "StatusChangedPayload")]
[JsonDerivedType(typeof(ItemDeletedPayloadDetails), "ItemDeletedPayload")]
[JsonDerivedType(typeof(ItemAddedPayloadDetails), "ItemAddedPayload")]
public abstract record PayloadBaseDetails;
