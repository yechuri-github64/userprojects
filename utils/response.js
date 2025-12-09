exports.ok = (payload) => ({ statusCode: 200, body: JSON.stringify(payload) });
exports.notFound = (payload) => ({ statusCode: 404, body: JSON.stringify(payload) });
