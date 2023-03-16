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
	name VARCHAR(50) NOT NULL,
	identifier VARCHAR(50) NOT NULL,
	description text NOT NULL,
	PRIMARY KEY (permission_id)
);

CREATE TABLE user_permissions
(
	user_id INTEGER NOT NULL,
	permission_id INTEGER NOT NULL,
	PRIMARY KEY(user_id, permission_id),
	FOREIGN KEY (user_id) REFERENCES users(user_id),
	FOREIGN KEY (permission_id) REFERENCES permissions(permission_id)
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
	PRIMARY KEY (form_id)
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
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_number_attributes
(
	element_id INTEGER NOT NULL,
	decimal_places INTEGER NOT NULL DEFAULT 0,
	min_value decimal NOT NULL DEFAULT 0,
	max_value decimal NOT NULL DEFAULT 0,
	is_summable TINYINT NOT NULL DEFAULT 0,
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
	regex_pattern text NOT NULL DEFAULT '',
	min_length INTEGER NOT NULL DEFAULT 0,
	max_length INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_text_attributes
(
	element_id INTEGER NOT NULL,
	regex_pattern text NOT NULL DEFAULT '',
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
	value_date DATE NOT NULL DEFAULT CURRENT_DATE,
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
	creation_date DATE NOT NULL DEFAULT CURRENT_DATE,
	creation_user_id INTEGER DEFAULT NULL,
	last_change DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	last_change_user_id INTEGER DEFAULT NULL,
	PRIMARY KEY (entry_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (creation_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
	FOREIGN KEY (last_change_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE TABLE form_entries_elements
(
	entry_id INTEGER NOT NULL,
	form_id INTEGER NOT NULL,
	element_id INTEGER NOT NULL,
	value_boolean TINYINT NOT NULL DEFAULT 0,
	value_string TEXT NOT NULL DEFAULT '',
	value_number DECIMAL NOT NULL DEFAULT 0,
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
	value_string VARCHAR(100) NOT NULL DEFAULT '',
	value_number DECIMAL NOT NULL DEFAULT 0,
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

/* DATA */
INSERT INTO permissions (name, identifier, description) VALUES 
('Form management','EDIT_FORMS','Allows the user the create, edit and delete new form templates.'),
('Submitted forms management','EDIT_ENTRIES','Allows the user the edit submitted form entries.'),
('User management','EDIT_USERS','Allows the user to manage the users.'),
('Delete forms','DELETE_FORMS','Allows the user to entire forms.'),
('Delete Entries','DELETE_ENTRIES','Allows the user to delete submitted form entries.');


