CREATE TABLE if not exists quotes (
    "date" date,
    "type" integer,
    symbol varchar(10),
    "open" numeric,
    "close" numeric,
    high numeric,
    low numeric, 
    volume numeric
);