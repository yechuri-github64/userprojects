module.exports = {
  Connection: function () {
    return {
      login: async () => {},
      sobject: function () {
        return {
          retrieve: async (id) => ({ Id: id, Name: `Account ${id}` }),
        };
      },
    };
  },
};
