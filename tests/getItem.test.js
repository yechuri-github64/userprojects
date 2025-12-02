const path = require('path');

describe('getItem function', () => {
  const functionPath = path.resolve(__dirname, '../src/functions/getItem/index.js');
  const helperPath = path.resolve(__dirname, '../src/helpers/salesforce/connection.js');

  beforeEach(() => {
    jest.resetModules();
    // Provide minimal Salesforce env vars so the real helper won't throw.
    process.env.SF_USERNAME = 'testuser';
    process.env.SF_PASSWORD = 'testpass';
    process.env.SF_TOKEN = '';
    // Insert a mock for the Salesforce connection module into require.cache
    const mockConn = {
      getConnection: async () => ({
        sobject: () => ({
          retrieve: async (id) => ({ Id: id, Name: 'Account ' + id })
        })
      })
    };
    require.cache[helperPath] = {
      id: helperPath,
      filename: helperPath,
      loaded: true,
      exports: mockConn
    };
  });

  afterEach(() => {
    delete require.cache[helperPath];
    delete process.env.SF_USERNAME;
    delete process.env.SF_PASSWORD;
    delete process.env.SF_TOKEN;
  });

  test('returns 400 when id missing', async () => {
    const fn = require(functionPath);
    const res = await fn({ params: {} }, {});
    expect(res.status).toBe(400);
    expect(res.jsonBody.success).toBe(false);
  });

  test('returns 200 and data when record found', async () => {
    const fn = require(functionPath);
    const res = await fn({ params: { id: 'ABC' } }, {});
    expect(res.status).toBe(200);
    expect(res.jsonBody.success).toBe(true);
    expect(res.jsonBody.data).toEqual({ Id: 'ABC', Name: 'Account ABC' });
  });
});
