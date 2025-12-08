%dw 2.0
output application/java
---
{
 travelcard_type: payload.travelcardType,
 travelcard_valid_from: payload.travelcardValidFrom,
 travelcard_valid_to: payload.travelcardValidTo,
 travelcard_name: payload.travelcardName default null,
 travelcard_number: payload.travelcardNumber,
 travelcard_requested_date: payload.travelcardRequestedDate,
 travelcard_transaction_reference: payload.travelcardTransactionReference,
 travelcard_usable_to: payload.travelcardUsableTo default null
}