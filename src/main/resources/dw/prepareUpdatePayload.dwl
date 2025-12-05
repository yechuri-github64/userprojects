%dw 2.0
output application/java
var src = vars.incoming default payload
---
{
 type: "Account",
 id: src.id,
 fields: {
 (if (src.name? ) then { Name: src.name } else {}),
 (if (src.email? ) then { Email__c: src.email } else {}),
 (if (src.address? ) then { Address__c: src.address } else {})
 }
}