using System.Text.Json.Serialization;

namespace Gateway.Models.Payloads;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "payloadType")]
[JsonDerivedType(typeof(OrderCreatedPayload), typeDiscriminator: "orderCreated")]
[JsonDerivedType(typeof(StatusChangedPayload), typeDiscriminator: "statusChanged")]
[JsonDerivedType(typeof(ItemAddedPayload), typeDiscriminator: "itemAdded")]
[JsonDerivedType(typeof(ItemDeletedPayload), typeDiscriminator: "itemDeleted")]
public abstract record HistoryPayloadBase;