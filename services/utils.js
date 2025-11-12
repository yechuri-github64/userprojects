function parseJsonSafe(str) {
  try { return JSON.parse(str); } catch (e) { return null; }
}
function stringifySafe(obj) {
  try { return JSON.stringify(obj); } catch (e) { return String(obj); }
}
module.exports = { parseJsonSafe, stringifySafe };