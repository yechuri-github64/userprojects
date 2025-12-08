%dw 2.0
output application/json
---
if (payload is Array) then
 payload map ((item) -> {
 id: item.id,
 travelcardType: item.travelcard_type,
 travelcardValidFrom: item.travelcard_valid_from,
 travelcardValidTo: item.travelcard_valid_to,
 travelcardName: item.travelcard_name default null,
 travelcardNumber: item.travelcard_number,
 travelcardRequestedDate: item.travelcard_requested_date,
 travelcardTransactionReference: item.travelcard_transaction_reference,
 travelcardUsableTo: item.travelcard_usable_to default null
 })
else
 {
 id: payload.id,
 travelcardType: payload.travelcard_type,
 travelcardValidFrom: payload.travelcard_valid_from,
 travelcardValidTo: payload.travelcard_valid_to,
 travelcardName: payload.travelcard_name default null,
 travelcardNumber: payload.travelcard_number,
 travelcardRequestedDate: payload.travelcard_requested_date,
 travelcardTransactionReference: payload.travelcard_transaction_reference,
 travelcardUsableTo: payload.travelcard_usable_to default null
 }