%dw 2.0
output application/json
---
{
 error: {
 message: error.description default "An error occurred",
 type: error.errorType as String default "UNKNOWN",
 cause: error.getMessage() default "no further details"
 }
}
