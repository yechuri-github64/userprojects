%dw 2.0
output application/java
---
// Expect incoming API payload with Name, BillingCity, BillingState
{
 Name: payload.Name default "",
 BillingCity: payload.BillingCity default null,
 BillingState: payload.BillingState default null
}
