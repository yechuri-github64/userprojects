%dw 2.0
output application/java
---
// Payload expected to be object with fields to update
// Attributes.uriParams.id contains the id
var id = attributes.uriParams.id default payload.Id default null
if (id == null) do {
 error('Missing id for update', 'VALIDATION')
}
---
{
 Id: id,
 // include only provided fields
 (if (payload.Name? ) then { Name: payload.Name } else {}),
 (if (payload.Phone? ) then { Phone: payload.Phone } else {}),
 (if (payload.Website? ) then { Website: payload.Website } else {}),
 (if (payload.BillingStreet? ) then { BillingStreet: payload.BillingStreet } else {}),
 (if (payload.BillingCity? ) then { BillingCity: payload.BillingCity } else {}),
 (if (payload.BillingState? ) then { BillingState: payload.BillingState } else {}),
 (if (payload.BillingPostalCode? ) then { BillingPostalCode: payload.BillingPostalCode } else {})
}
