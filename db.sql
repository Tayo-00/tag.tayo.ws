CREATE TABLE maps (
    id          serial,
    mapset_id   integer NOT NULL,
    map_id      integer NOT NULL,
    players     integer NOT NULL,
    combined_sr varchar NOT NULL,
    player_sr   text NOT NULL,
    title       varchar NOT NULL,
    mapper      varchar NOT NULL,
    artist      varchar NOT NULL,
    difficulty  varchar NOT NULL,
    notelock    boolean,
    date_added  timestamp WITHOUT time zone DEFAULT now()
);

INSERT INTO maps (mapset_id, map_id, players, combined_sr, player_sr, title, mapper, artist, difficulty, notelock) VALUES
(7385, 35015, 4, '9.71', 'json object here', 'Utage wa Eien ni ~SHD~', 'DJPop', 'IOSYS', 'TAG4', false),
(295458, 663776, 4, '10.39', 'json object here', 'Yakumo ~ JOINT STRUGGLE', 'MillhioreF', 'Kucchy vs Akky', 'TAG4', null),
(695140, 1472969, 4, '7.42', 'json object here', 'Ineffabilis', 'YokesPai', 'Gentle Stick X M2U', 'TAG4', true);