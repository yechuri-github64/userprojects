%dw 2.0
output application/json
---
// Transform various Salesforce connector responses into unified JSON
if (payload == null) then { result: [] }
else if (payload is Array) then
 { result: payload map ((p) ->
 if (p containsKey 'Id') then { id: p.Id as String, status: "created" } else p
 ) }
else if (payload containsKey 'Id') then { result: { id: payload.Id as String } }
else if (payload containsKey 'records') then { result: payload.records }
else payload
