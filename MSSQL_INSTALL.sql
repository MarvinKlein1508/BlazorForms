CREATE TABLE users
(
	user_id INTEGER IDENTITY(1,1) NOT NULL,
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
	permission_id INTEGER IDENTITY(1,1) NOT NULL,
	name VARCHAR(50) NOT NULL,
	identifier VARCHAR(50) NOT NULL,
	description text NOT NULL,
	PRIMARY KEY(permission_id)
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
	form_id INTEGER IDENTITY(1,1) NOT NULL,
	name VARCHAR(50) NOT NULL,
	login_required TINYINT NOT NULL DEFAULT 0,
	is_active TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY (form_id)
);

CREATE TABLE form_rows
(
	row_id INTEGER IDENTITY(1,1) NOT NULL,
	form_id INTEGER NOT NULL,
	is_active TINYINT NOT NULL DEFAULT 0,
	sort_order INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY (row_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_columns
(
	column_id INTEGER IDENTITY(1,1) NOT NULL,
	form_id INTEGER NOT NULL,
	row_id INTEGER NOT NULL,
	is_active TINYINT NOT NULL DEFAULT 0,
	sort_order INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY (column_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id),
	FOREIGN KEY (row_id) REFERENCES form_rows(row_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements
(
	element_id INTEGER IDENTITY(1,1) NOT NULL,
	form_id INTEGER NOT NULL,
	row_id INTEGER NOT NULL,
	column_id INTEGER NOT NULL,
	guid VARCHAR(36) NOT NULL,
	name text NOT NULL,
	type VARCHAR(20) NOT NULL,
	is_active TINYINT NOT NULL DEFAULT 0,
	is_required TINYINT NOT NULL DEFAULT 0,
	sort_order INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id),
	FOREIGN KEY (row_id) REFERENCES form_rows(row_id),
	FOREIGN KEY (column_id) REFERENCES form_columns(column_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_options
(
	element_option_id INTEGER IDENTITY(1,1) NOT NULL,
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
	accept_file_types text NOT NULL DEFAULT '',
	min_size INTEGER NOT NULL DEFAULT 0,
	max_size INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_label_attributes
(
	element_id INTEGER NOT NULL,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE form_elements_number_attributes
(
	element_id INTEGER NOT NULL,
	decimal_places INTEGER NOT NULL DEFAULT 0,
	min_value decimal NOT NULL DEFAULT 0,
	max_value decimal NOT NULL DEFAULT 0,
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

