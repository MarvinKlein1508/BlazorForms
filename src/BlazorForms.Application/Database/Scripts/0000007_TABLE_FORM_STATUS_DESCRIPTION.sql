CREATE TABLE form_status_description
(
    status_id INTEGER NOT NULL,
    language_id INTEGER NOT NULL,
    name VARCHAR(255) NOT NULL DEFAULT '',
    description varchar(255) NOT NULL DEFAULT '',
    CONSTRAINT pk_form_status_description PRIMARY KEY(status_id, language_id),
    CONSTRAINT fk_form_status_description_form_status FOREIGN KEY(status_id) REFERENCES form_status(status_id),
    CONSTRAINT fk_form_status_description_language FOREIGN KEY(language_id) REFERENCES languages(language_id)
);