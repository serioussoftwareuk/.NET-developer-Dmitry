CREATE TABLE if not exists quotes (
    "date" date NOT NULL,
    symbol varchar(10) NOT NULL,
    "type" integer NOT NULL,
    "open" numeric,
    "close" numeric,
    high numeric,
    low numeric, 
    volume numeric,
    PRIMARY KEY("date", symbol, "type")
);