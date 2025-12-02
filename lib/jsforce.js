// Local stub used by Jest's moduleNameMapper so `require('jsforce')` resolves
// during tests without installing the real package.
module.exports = {
	Connection: function () {
		return {
			login: async () => {},
			sobject: function () {
				return {
					retrieve: async (id) => ({ Id: id, Name: `Account ${id}` }),
					find: function (filter, fields) {
						const self = this;
						self._filter = filter;
						self._fields = fields;
						return {
							limit(n) {
								this._limit = n;
								return this;
							},
							execute: async function () {
								// return a small fake result honoring Id/Name if requested
								return [
									{ Id: '001xx000003NGsYAAW', Name: 'Sample Account', Industry: 'Technology' },
								];
							},
						};
					},
				};
			},
		};
	},
};
