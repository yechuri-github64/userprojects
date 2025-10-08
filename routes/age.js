'use strict';

const express = require('express');
const controller = require('../controllers/ageController');

let router;
try {
  router = express.Router();
  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
  router = require('express').Router();
}

try {
  router.get('/', async (req, res, next) => {
    try {
      await controller.getAge(req, res, next);
    } catch (err) {
      console.error(" Failed", err);
      next(err);
    }
  });

  router.post('/', async (req, res, next) => {
    try {
      await controller.postAge(req, res, next);
    } catch (err) {
      console.error(" Failed", err);
      next(err);
    }
  });

  console.log(" Connected");
} catch (err) {
  console.error(" Failed", err);
}

module.exports = router;
