module.exports = {
  success: (data) => ({ success: true, data }),
  error: (message, code = "internal_error") => ({
    success: false,
    error: { message, code },
  }),
};
