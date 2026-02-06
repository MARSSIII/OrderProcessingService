using System.Text.Json.Serialization;

namespace Domain.Orders.Payloads;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(OrderCreatedByPayload), "OrderCreatedPayload")]
[JsonDerivedType(typeof(StatusChangedPayload), "StatusChangedPayload")]
[JsonDerivedType(typeof(ItemDeletedPayload), "ItemDeletedPayload")]
[JsonDerivedType(typeof(ItemAddedPayload), "ItemAddedPayload")]
public abstract record PayloadBase;
