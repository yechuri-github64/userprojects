function pruneEmpty(value) {
  if (value === null || value === undefined) return undefined;
  if (typeof value === 'string') {
    const v = value.trim();
    return v === '' ? undefined : v;
  }
  if (Array.isArray(value)) {
    const arr = value.map(pruneEmpty).filter((v) => v !== undefined);
    return arr.length ? arr : undefined;
  }
  if (typeof value === 'object') {
    const out = {};
    for (const key of Object.keys(value)) {
      const v = pruneEmpty(value[key]);
      if (v !== undefined) out[key] = v;
    }
    return Object.keys(out).length ? out : undefined;
  }
  return value;
}

module.exports = { pruneEmpty };
