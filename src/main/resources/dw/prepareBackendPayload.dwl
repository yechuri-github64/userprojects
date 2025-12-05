%dw 2.0
output application/json
---
// Expecting payload as array of objects [{id,name,email,address}, ...]
(payload default []) map ((item, index) -> {
 attributes: {
 type: "Account"
 },
 // Salesforce composite sObjects expect fields object
 fields: {
 Name: item.name default null,
 Email__c: item.email default null,
 Address__c: item.address default null
 }
})