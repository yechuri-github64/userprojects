%dw 2.0
output application/json
---
{
 error: {
 message: payload.message default "Unexpected error",
 details: payload.details default {}
 }
}
