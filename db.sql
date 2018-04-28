CREATE TABLE maps (
    id          serial,
    mapset_id   integer NOT NULL,
    map_id      integer NOT NULL,
    players     integer NOT NULL,
    combined_sr char NOT NULL,
    player_sr   char NOT NULL,
    title       char NOT NULL,
    mapper      char NOT NULL,
    difficulty  char NOT NULL,
    notelock    boolean,
    date_added  date
);