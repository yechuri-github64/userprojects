module.exports.retry = async function(fn, maxRetries = 3, delaySeconds = 3) {
  let lastErr;
  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      if (process.env.SALESFORCE_ENABLE_LOGGING === 'true') console.log(`Attempt ${attempt} of ${maxRetries}`);
      return await fn();
    } catch (err) {
      lastErr = err;
      console.log(`Attempt ${attempt} failed:`, err.message || err);
      if (attempt < maxRetries) await new Promise(r => setTimeout(r, delaySeconds * 1000));
    }
  }
  throw lastErr;
};
