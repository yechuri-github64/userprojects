%dw 2.0
output application/java
---
{
 id: vars.id as Number,
 travelcard_type: payload.travelcardType default null,
 travelcard_valid_from: payload.travelcardValidFrom default null,
 travelcard_valid_to: payload.travelcardValidTo default null,
 travelcard_name: payload.travelcardName default null,
 travelcard_number: payload.travelcardNumber default null,
 travelcard_requested_date: payload.travelcardRequestedDate default null,
 travelcard_transaction_reference: payload.travelcardTransactionReference default null,
 travelcard_usable_to: payload.travelcardUsableTo default null
}