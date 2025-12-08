%dw 2.0
output application/json
---
// Transform Salesforce response recordset to API response

fun toResp(rec) = {
 id: rec.Id default rec.id default null,
 Name: rec.Name default null,
 BillingCity: rec.BillingCity default null,
 BillingState: rec.BillingState default null
}

if (payload is Array) then payload map toResp else if (payload is Object) then toResp(payload) else payload
