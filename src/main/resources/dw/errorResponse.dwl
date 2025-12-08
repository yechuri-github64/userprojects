%dw 2.0
output application/json
---
{
 error: {
 type: payload.type default "ServerError",
 description: payload.description default "An unexpected error occurred"
 }
}