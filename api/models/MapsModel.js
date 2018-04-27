'use strict';

// require database
const postgres = require('../database/connections/postgres');

let MapsModel = {};

// Get all maps
MapsModel.getMaps = function(callback) {
    postgres.task(async (db) => {
        try {
            const maps = await db.any('SELECT * FROM maps');
            if (maps[0]) {
                callback({
                    status: 200,
                    data: maps,
                });
            } else {
                callback({
                    status: 404,
                    data: {
                        error: 'Could not find any records in database',
                    },
                });
            }
        } catch (e) {
            callback({
                status: 500,
                data: {
                    error: 'Could not fetch data from database',
                },
            });
        }
    });
};

module.exports = MapsModel;
