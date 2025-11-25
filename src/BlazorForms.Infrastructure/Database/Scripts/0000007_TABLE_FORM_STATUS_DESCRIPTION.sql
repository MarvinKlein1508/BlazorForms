CREATE TABLE form_status_description
(
    status_id INTEGER NOT NULL,
    code VARCHAR(5) NOT NULL,
    name VARCHAR(255) NOT NULL DEFAULT '',
    description varchar(255) NOT NULL DEFAULT '',
    CONSTRAINT pk_form_status_description PRIMARY KEY(status_id, code),
    CONSTRAINT fk_form_status_description_form_status FOREIGN KEY(status_id) REFERENCES form_status(status_id),
    CONSTRAINT fk_form_status_description_code FOREIGN KEY(code) REFERENCES languages(code)
);