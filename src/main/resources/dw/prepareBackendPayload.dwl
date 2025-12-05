%dw 2.0
output application/java
---
(payload default []) map (item) -> {
 type: "Account",
 fields: {
 Name: item.name,
 Email__c: item.email,
 Address__c: item.address
 }
}