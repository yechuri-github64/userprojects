%dw 2.0
output application/java
---
// Merge path param id into payload for update
var id = attributes.uriParams.id default null
---
{
 Id: id,
 Name: payload.Name default null,
 BillingCity: payload.BillingCity default null,
 BillingState: payload.BillingState default null
}
