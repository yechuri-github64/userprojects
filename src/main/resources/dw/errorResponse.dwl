%dw 2.0
output application/json
---
{
 error: {
 message: error.description default "Internal server error",
 type: error.errorType default "UNKNOWN",
 cause: error.cause default null
 }
}
