%dw 2.0
output application/json
---
// Expecting payload as single object {name?,email?,address?}
var body = payload default {}
---
// Only include provided fields
body reduce ((value, acc = {}) -> do {
 var entries = (value pluck ((v,k) -> { (k): v }))
 ---
 entries
}) mapObject ((v,k) -> 
 // map input keys to Salesforce fields
 if (k == "name") ("Name") : v
 else if (k == "email") ("Email__c") : v
 else if (k == "address") ("Address__c") : v
 else (k): v
)
