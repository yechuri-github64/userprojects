%dw 2.0
output application/json
---
if (payload is Array) then
 payload map ((item) -> {
 id: item.id default item.Id,
 success: item.success default (item.successful default false),
 errors: item.errors default []
 })
else if (payload is Object and payload.records? ) then
 (
 (payload.records map ((r) -> {
 id: r.Id,
 name: r.Name,
 email: r.Email__c,
 address: r.Address__c
 }))
 )
else
 (
 if (payload.Id?) then {
 id: payload.Id,
 name: payload.Name,
 email: payload.Email__c,
 address: payload.Address__c
 } else payload
 )