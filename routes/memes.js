const express = require('express');
try {
  const router = express.Router();
  const memesController = require('../controllers/memesController');
  // GET /api/meme -> returns a random meme
  router.get('/meme', memesController.getRandomMeme);
  module.exports = router;
  console.log(" Connected");
} catch(err) {
  console.error(" Failed", err);
}
