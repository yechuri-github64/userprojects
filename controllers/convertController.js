try {
  // Controller to handle currency conversion requests
  module.exports = {
    convert: (req, res) => {
      try {
        const rawAmount = (req.query && req.query.amount) || (req.body && req.body.amount);
        const fromCurrency = ((req.query && req.query.fromCurrency) || (req.body && req.body.fromCurrency) || process.env.BASE_CURRENCY || 'USD').toString().toUpperCase();
        const toCurrency = ((req.query && req.query.toCurrency) || (req.body && req.body.toCurrency) || process.env.TARGET_CURRENCY || 'INR').toString().toUpperCase();

        if (rawAmount === undefined || rawAmount === null || rawAmount === '') {
          const error = { message: 'Missing required query parameter or body field: amount', code: 'MISSING_PARAMETER', details: { parameter: 'amount' } };
          console.error('Failed', error);
          return res.status(400).json({ error });
        }

        const amount = Number(rawAmount);
        if (Number.isNaN(amount)) {
          const error = { message: 'Invalid amount: must be a number', code: 'INVALID_PARAMETER', details: { parameter: 'amount', value: rawAmount } };
          console.error('Failed', error);
          return res.status(400).json({ error });
        }

        // Support only USD -> INR using environment exchange rate for simplicity
        let rate;
        if (fromCurrency === 'USD' && toCurrency === 'INR') {
          rate = Number(process.env.EXCHANGE_RATE_USD_INR || process.env.EXCHANGE_RATE || '82.5');
        } else if (fromCurrency === toCurrency) {
          rate = 1;
        } else {
          const error = { message: 'Conversion rate not available for currency pair', code: 'RATE_NOT_AVAILABLE', details: { fromCurrency, toCurrency } };
          console.error('Failed', error);
          return res.status(400).json({ error });
        }

        const convertedAmount = Number((amount * rate).toFixed(6));

        const output = {
          fromCurrency,
          toCurrency,
          amount,
          rate,
          convertedAmount
        };

        console.log('Connected');
        return res.json(output);
      } catch (err) {
        console.error('Failed', err);
        return res.status(500).json({ error: { message: 'Internal server error', code: 'INTERNAL_ERROR', details: err && err.message ? err.message : err } });
      }
    }
  };

  console.log('Connected');
} catch (err) {
  console.error('Failed', err);
  module.exports = { convert: (req, res) => res.status(500).json({ error: { message: 'Controller failed to load', code: 'CONTROLLER_LOAD_ERROR', details: err && err.message ? err.message : err } }) };
}
