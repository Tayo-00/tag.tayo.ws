CREATE TABLE maps (
    id          serial,
    mapset_id   integer,
    map_id      integer,
    players     integer,
    combined_sr char,
    player_sr   char,
    title       char,
    difficulty  char,
    date_added  date
);