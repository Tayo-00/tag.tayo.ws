'use strict';

const express = require('express');
const router = express.Router();

// Require maps model
const MapsModel = require('../models/MapsModel');

// Maps router
router.get('/', function(req, res, next) {
  MapsModel.getMaps(function(response) {
    return res.status(response.status).send(response.data);
  });
});

module.exports = router;
