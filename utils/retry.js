module.exports = async function retry(fn, maxAttempts, delayMs) {
  const attempts = Math.max(1, parseInt(maxAttempts || 1, 10));
  const delay = parseInt(delayMs || 0, 10);
  let lastError;
  for (let i = 0; i < attempts; i++) {
    try {
      return await fn();
    } catch (err) {
      lastError = err;
      const isLast = i === attempts - 1;
      console.error(`Retry attempt ${i + 1} failed`, err && err.message ? err.message : err);
      if (isLast) break;
      if (delay > 0) await new Promise((r) => setTimeout(r, delay));
    }
  }
  throw lastError;
};
