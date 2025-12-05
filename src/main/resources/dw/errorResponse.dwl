%dw 2.0
output application/json
---
{
 error: {
 message: vars.errorMessage default "Unexpected error",
 type: vars.errorType default "APPLICATION",
 detail: vars.errorDetail default null
 }
}