function isEmptyValue(val) {
  return val === null || val === undefined || (typeof val === 'string' && val.trim() === '');
}

function removeEmptyFields(input) {
  if (Array.isArray(input)) {
    const arr = input.map(removeEmptyFields).filter((v) => !(isEmptyValue(v) || (typeof v === 'object' && Object.keys(v).length === 0)));
    return arr;
  }

  if (input && typeof input === 'object') {
    const out = {};
    for (const key of Object.keys(input)) {
      const val = input[key];
      if (isEmptyValue(val)) continue;
      if (typeof val === 'object') {
        const cleaned = removeEmptyFields(val);
        if (cleaned === null || cleaned === undefined) continue;
        if (typeof cleaned === 'object' && Object.keys(cleaned).length === 0) continue;
        out[key] = cleaned;
      } else {
        out[key] = val;
      }
    }
    return out;
  }

  return input;
}

module.exports = { removeEmptyFields };
