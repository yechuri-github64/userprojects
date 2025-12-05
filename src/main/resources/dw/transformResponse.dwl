%dw 2.0
output application/json
---
// Transform Salesforce composite sobjects create response to simple JSON
var resp = payload default {}
---
{
 results: (resp.results default []) map ((r) -> {
 id: r.id when (r.id? default false) otherwise null,
 success: r.success default false,
 errors: (r.errors default [])
 })
}
