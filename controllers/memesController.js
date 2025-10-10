try {
  const memesModel = require('../models/memes');
  async function getRandomMeme(req, res, next) {
    try {
      const meme = memesModel.getRandom();
      if (!meme) {
        const err = { status: 404, message: 'No memes available' };
        console.error(" Failed", err);
        return next(err);
      }
      res.json(meme);
    } catch(err) {
      console.error(" Failed", err);
      next({ status: 500, message: 'Failed to get meme', details: err && err.message });
    }
  }
  module.exports = { getRandomMeme };
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
