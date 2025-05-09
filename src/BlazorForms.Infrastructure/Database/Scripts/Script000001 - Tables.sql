CREATE TABLE languages 
(
    language_id INTEGER GENERATED ALWAYS AS IDENTITY,
    name VARCHAR(32) NOT NULL,
    code VARCHAR(5) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    status BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT pk_languages PRIMARY KEY (language_id),
    CONSTRAINT uq_languages_code UNIQUE (code)
);

INSERT INTO languages (name, code, sort_order, status) VALUES 
('Deutsch', 'de', 0, TRUE),
('English', 'en', 1, TRUE);

CREATE TABLE users
(
    user_id INTEGER GENERATED ALWAYS AS IDENTITY,
    username VARCHAR(50) NOT NULL,
    display_name VARCHAR(100) NOT NULL,
    active_directory_guid UUID,
    email VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    origin VARCHAR(5) NOT NULL,
    CONSTRAINT pk_users PRIMARY KEY(user_id)
);

CREATE TABLE permissions
(
    permission_id INTEGER GENERATED ALWAYS AS IDENTITY,
    identifier VARCHAR(50) NOT NULL,
    CONSTRAINT pk_permissions PRIMARY KEY (permission_id)
);

INSERT INTO permissions (identifier) VALUES 
('EDIT_FORMS'),
('EDIT_ENTRIES'),
('EDIT_USERS'),
('DELETE_FORMS'),
('DELETE_ENTRIES'),
('EDIT_STATUS');