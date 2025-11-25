CREATE TABLE configs
(
    code VARCHAR(128) NOT NULL,
    default_language_id INTEGER NOT NULL,
    CONSTRAINT pk_config PRIMARY KEY(code),
    CONSTRAINT fk_default_language FOREIGN KEY(default_language_id) REFERENCES languages(language_id)
);
