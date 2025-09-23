const db = require('../config/db');
jest.mock('../config/db');

const { ordermanagement } = require('../index');

describe('ordermanagement Lambda', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('saves order successfully', async () => {
    db.query.mockResolvedValue({ insertId: 123 });

    const event = { body: JSON.stringify({ customername: 'Alice', id: 'ord-100', order_amount: 49.99, quantity: 2, subscription: true }) };

    const res = await ordermanagement(event);

    expect(res.statusCode).toBe(201);
    const body = JSON.parse(res.body);
    expect(body.message).toBe('Order saved');
    expect(body.data.insertedId).toBe(123);
    expect(db.query).toHaveBeenCalledTimes(1);
  });

  test('returns validation error for missing fields', async () => {
    const event = { body: JSON.stringify({ customername: 'Bob' }) };
    const res = await ordermanagement(event);
    expect(res.statusCode).toBe(400);
    const body = JSON.parse(res.body);
    expect(body.error).toBeDefined();
    expect(body.error.details).toBeDefined();
  });

  test('handles DB errors gracefully', async () => {
    db.query.mockRejectedValue(new Error('DB down'));

    const event = { body: JSON.stringify({ customername: 'Charlie', id: 'ord-200', order_amount: 10, quantity: 1, subscription: 'false' }) };

    const res = await ordermanagement(event);
    expect(res.statusCode).toBe(500);
    const body = JSON.parse(res.body);
    expect(body.error).toBeDefined();
    expect(body.error.message).toContain('DB down');
  });
});
