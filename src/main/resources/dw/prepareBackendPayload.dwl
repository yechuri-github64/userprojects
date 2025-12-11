%dw 2.0
output application/java
---
// Expecting incoming payload to be a JSON array of accounts or a single object
var inputPayload = payload
---
if (inputPayload is Array) then
 inputPayload map ((item) ->
 {
 Name: item.Name default null,
 Phone: item.Phone default null,
 Website: item.Website default null,
 BillingStreet: item.BillingStreet default null,
 BillingCity: item.BillingCity default null,
 BillingState: item.BillingState default null,
 BillingPostalCode: item.BillingPostalCode default null
 }
 )
else
 // single object -> wrap into array
 [
 {
 Name: inputPayload.Name default null,
 Phone: inputPayload.Phone default null,
 Website: inputPayload.Website default null,
 BillingStreet: inputPayload.BillingStreet default null,
 BillingCity: inputPayload.BillingCity default null,
 BillingState: inputPayload.BillingState default null,
 BillingPostalCode: inputPayload.BillingPostalCode default null
 }
 ]
