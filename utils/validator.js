module.exports = {
  parseDOB: (input) => {
    if (!input) {
      const err = new Error('DOB not provided');
      err.statusCode = 400;
      throw err;
    }
    let date;
    if (Object.prototype.toString.call(input) === '[object Date]') {
      date = input;
    } else if (typeof input === 'number') {
      date = new Date(input);
    } else if (typeof input === 'string') {
      date = new Date(input);
    } else if (typeof input === 'object' && input.date) {
      date = new Date(input.date);
    } else {
      const err = new Error('Invalid DOB format');
      err.statusCode = 400;
      throw err;
    }
    if (Number.isNaN(date.getTime())) {
      const err = new Error('Invalid DOB value');
      err.statusCode = 400;
      throw err;
    }
    return date;
  }
};