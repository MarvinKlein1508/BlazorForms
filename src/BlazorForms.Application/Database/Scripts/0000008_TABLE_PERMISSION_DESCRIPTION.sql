CREATE TABLE permission_description
(
    permission_id INTEGER NOT NULL,
    language_id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL,
    description text NOT NULL,
    CONSTRAINT pk_permission_description PRIMARY KEY(permission_id, language_id),
    CONSTRAINT fk_permission_description_permission FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_permission_description_language FOREIGN KEY (language_id) REFERENCES languages(language_id) ON DELETE CASCADE ON UPDATE CASCADE
);