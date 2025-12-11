const validator = require('../utils/validator');
const timeUtils = require('../utils/timeUtils');
module.exports = {
  calculateFromDOB: async (dobInput) => {
    try {
      const dob = validator.parseDOB(dobInput);
      const now = new Date();
      if (dob.getTime() > now.getTime()) {
        const err = new Error('DOB is in the future');
        err.statusCode = 400;
        throw err;
      }
      const msDiff = now.getTime() - dob.getTime();
      const seconds = Math.floor(msDiff / timeUtils.MS_PER_SECOND);
      const minutes = Math.floor(msDiff / timeUtils.MS_PER_MINUTE);
      const days = Math.floor(msDiff / timeUtils.MS_PER_DAY);
      const weeks = Math.floor(days / 7);
      const message = `${days} day${days === 1 ? '' : 's'}, ${weeks} week${weeks === 1 ? '' : 's'}, ${minutes} minute${minutes === 1 ? '' : 's'}, ${seconds} second${seconds === 1 ? '' : 's'}`;
      return {
        dob: dob.toISOString().split('T')[0],
        calculatedAt: now.toISOString(),
        days,
        weeks,
        minutes,
        seconds,
        message
      };
    } catch (err) {
      throw err;
    }
  }
};