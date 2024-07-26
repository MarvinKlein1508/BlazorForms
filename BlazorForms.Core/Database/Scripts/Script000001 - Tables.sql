CREATE TABLE languages
(
    language_id INTEGER NOT NULL AUTO_INCREMENT,
    name varchar(32) NOT NULL,
    code varchar(5) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    status TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY(language_id),
    UNIQUE(code)
);

CREATE TABLE users
(
    user_id INTEGER NOT NULL AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    display_name VARCHAR(100) NOT NULL,
    active_directory_guid VARCHAR(36),
    email VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    origin VARCHAR(5) NOT NULL,
    PRIMARY KEY(user_id)
);

CREATE TABLE permissions
(
    permission_id INTEGER NOT NULL AUTO_INCREMENT,
    identifier VARCHAR(50) NOT NULL,
    PRIMARY KEY (permission_id)
);

CREATE TABLE permission_description
(
    permission_id INTEGER NOT NULL,
    code VARCHAR(5) NOT NULL DEFAULT '',
    name VARCHAR(50) NOT NULL,
    description text NOT NULL,
    PRIMARY KEY(permission_id, code),
    FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE user_permissions
(
    user_id INTEGER NOT NULL,
    permission_id INTEGER NOT NULL,
    PRIMARY KEY(user_id, permission_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE user_filter
(
    user_id INTEGER NOT NULL,
    filter_type VARCHAR(255) NOT NULL,
    page VARCHAR(255) NOT NULL,
    serialized longtext NOT NULL CHECK (json_valid(serialized)),
    PRIMARY KEY (user_id, filter_type, page),
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_status
(
    status_id INTEGER NOT NULL AUTO_INCREMENT,
    requires_approval TINYINT NOT NULL DEFAULT 0,
    is_completed TINYINT NOT NULL DEFAULT 0,
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY(status_id)
);

CREATE TABLE form_status_description
(
    status_id INTEGER NOT NULL,
    code VARCHAR(5) NOT NULL,
    name VARCHAR(50) NOT NULL,
    description varchar(255) NOT NULL DEFAULT '',
    PRIMARY KEY (status_id, code),
    FOREIGN KEY (status_id) REFERENCES form_status(status_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE forms
(
    form_id INTEGER NOT NULL AUTO_INCREMENT,
    name VARCHAR(50) NOT NULL,
    description TEXT NOT NULL,
    logo LONGBLOB NOT NULL,
    image LONGBLOB NOT NULL,
    login_required TINYINT NOT NULL DEFAULT 0,
    is_active TINYINT NOT NULL DEFAULT 0,
    default_status_id INTEGER NOT NULL,
    language_id INTEGER NOT NULL,
    default_name VARCHAR(50) NOT NULL DEFAULT '',
    PRIMARY KEY (form_id),
    FOREIGN KEY (default_status_id) REFERENCES form_status(status_id),
    FOREIGN KEY (language_id) REFERENCES languages(language_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_to_user
(
    form_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    PRIMARY KEY(form_id, user_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_managers
(
    form_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    receive_email TINYINT NOT NULL DEFAULT 0,
    can_approve TINYINT NOT NULL DEFAULT 0,
    status_change_notification_default TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY(form_id, user_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_rows
(
    row_id INTEGER NOT NULL AUTO_INCREMENT,
    form_id INTEGER NOT NULL,
    is_active TINYINT NOT NULL DEFAULT 0,
    rule_type VARCHAR(20) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (row_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_columns
(
    column_id INTEGER NOT NULL AUTO_INCREMENT,
    form_id INTEGER NOT NULL,
    row_id INTEGER NOT NULL,
    is_active TINYINT NOT NULL DEFAULT 0,
    rule_type VARCHAR(20) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (column_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (row_id) REFERENCES form_rows(row_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements
(
    element_id INTEGER NOT NULL AUTO_INCREMENT,
    form_id INTEGER NOT NULL,
    row_id INTEGER NOT NULL,
    column_id INTEGER NOT NULL,
    table_parent_element_id INTEGER NOT NULL DEFAULT 0,
    guid VARCHAR(36) NOT NULL,
    name text NOT NULL,
    type VARCHAR(20) NOT NULL,
    is_active TINYINT NOT NULL DEFAULT 0,
    is_required TINYINT NOT NULL DEFAULT 0,
    reset_on_copy TINYINT NOT NULL DEFAULT 0,
    rule_type VARCHAR(20) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (row_id) REFERENCES form_rows(row_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (column_id) REFERENCES form_columns(column_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_options
(
    element_option_id INTEGER NOT NULL AUTO_INCREMENT,
    element_id INTEGER NOT NULL,
    name VARCHAR(100) NOT NULL,
    PRIMARY KEY(element_option_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_checkbox_attributes
(
    element_id INTEGER NOT NULL,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_date_attributes
(
    element_id INTEGER NOT NULL,
    is_current_date_default TINYINT NOT NULL DEFAULT 0,
    min_value date,
    max_value date,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_file_attributes
(
    element_id INTEGER NOT NULL,
    min_size INTEGER NOT NULL DEFAULT 0,
    max_size INTEGER NOT NULL DEFAULT 0,
    allow_multiple_files TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_file_types
(
    element_id INTEGER NOT NULL,
    content_type VARCHAR(255) NOT NULL,
    PRIMARY KEY(element_id, content_type),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_label_attributes
(
    element_id INTEGER NOT NULL,
    description TEXT NOT NULL,
    show_on_pdf TINYINT NOT NULL DEFAULT 1,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_number_attributes
(
    element_id INTEGER NOT NULL,
    decimal_places INTEGER NOT NULL DEFAULT 0,
    min_value decimal(15,5) NOT NULL DEFAULT 0,
    max_value decimal(15,5) NOT NULL DEFAULT 0,
    is_summable TINYINT NOT NULL DEFAULT 0,
    default_value decimal NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_radio_attributes
(
    element_id INTEGER NOT NULL,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_select_attributes
(
    element_id INTEGER NOT NULL,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_table_attributes
(
    element_id INTEGER NOT NULL,
    allow_add_rows TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_textarea_attributes
(
    element_id INTEGER NOT NULL,
    regex_pattern text,
    regex_validation_message VARCHAR(150) NOT NULL DEFAULT '',
    min_length INTEGER NOT NULL DEFAULT 0,
    max_length INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_text_attributes
(
    element_id INTEGER NOT NULL,
    regex_pattern text,
    regex_validation_message VARCHAR(150) NOT NULL DEFAULT '',
    min_length INTEGER NOT NULL DEFAULT 0,
    max_length INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY(element_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_rules
(
    rule_id INTEGER NOT NULL AUTO_INCREMENT,
    form_id INTEGER NOT NULL,
    row_id INTEGER NOT NULL,
    column_id INTEGER,
    element_id INTEGER,
    logical_operator varchar(10) NOT NULL,
    element_guid VARCHAR(36) NOT NULL,
    comparison_operator VARCHAR(20) NOT NULL,
    value_boolean TINYINT NOT NULL DEFAULT 0,
    value_string VARCHAR(100) NOT NULL DEFAULT '',
    value_number DECIMAL NOT NULL DEFAULT 0,
    value_date DATE NOT NULL DEFAULT (CURRENT_DATE),
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (rule_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (row_id) REFERENCES form_rows(row_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (column_id) REFERENCES form_columns(column_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_number_calc_rules
(
    calc_rule_id INTEGER NOT NULL AUTO_INCREMENT,
    element_id INTEGER NOT NULL,
    math_operator VARCHAR(20) NOT NULL,
    guid_element VARCHAR(36) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY(calc_rule_id),
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_entries
(
    entry_id INTEGER NOT NULL AUTO_INCREMENT,
    form_id INTEGER NOT NULL,
    name VARCHAR(50) NOT NULL DEFAULT '',
    creation_date DATE NOT NULL DEFAULT (CURRENT_DATE),
    creation_user_id INTEGER DEFAULT NULL,
    last_change DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_change_user_id INTEGER DEFAULT NULL,
    status_id INTEGER NOT NULL,
    approved TINYINT NOT NULL DEFAULT 0,
    priority INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (entry_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (creation_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (last_change_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (status_id) REFERENCES form_status(status_id)
);

CREATE TABLE form_entries_elements
(
    entry_id INTEGER NOT NULL,
    form_id INTEGER NOT NULL,
    element_id INTEGER NOT NULL,
    value_boolean TINYINT NOT NULL DEFAULT 0,
    value_string TEXT,
    value_number DECIMAL(15,5) NOT NULL DEFAULT 0,
    value_date DATE DEFAULT NULL,
    PRIMARY KEY (entry_id, form_id, element_id),
    FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_entries_table_elements
(
    table_row_number INTEGER NOT NULL,
    table_parent_element_id INTEGER NOT NULL,
    entry_id INTEGER NOT NULL,
    element_id INTEGER NOT NULL,
    value_boolean TINYINT NOT NULL DEFAULT 0,
    value_string TEXT,
    value_number DECIMAL(15,5) NOT NULL DEFAULT 0,
    value_date DATE DEFAULT NULL,
    PRIMARY KEY (table_row_number, table_parent_element_id, entry_id, element_id),
    FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_entries_files
(
    file_id INTEGER NOT NULL AUTO_INCREMENT,
    entry_id INTEGER NOT NULL,
    element_id INTEGER NOT NULL,
    data LONGBLOB NOT NULL,
    content_type VARCHAR(50),
    filename VARCHAR(255),
    PRIMARY KEY(file_id),
    FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_entry_history
(
    history_id INTEGER NOT NULL AUTO_INCREMENT,
    entry_id INTEGER NOT NULL,
    status_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    comment TEXT,
    date_added DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (history_id),
    FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (status_id) REFERENCES form_status(status_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_entry_history_notify 
(
    history_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    notify TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY (history_id, user_id),
    FOREIGN KEY (history_id) REFERENCES form_entry_history(history_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE notifications
(
    notification_id INTEGER NOT NULL AUTO_INCREMENT,
    created DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    user_id INTEGER DEFAULT NULL,
    icon VARCHAR(50) DEFAULT NULL,
    title VARCHAR(100) NOT NULL,
    details TEXT NOT NULL,
    href VARCHAR(100) NOT NULL,
    is_read TINYINT NOT NULL DEFAULT 0,
    read_timestamp DATETIME DEFAULT NULL,
    PRIMARY KEY (notification_id)
);