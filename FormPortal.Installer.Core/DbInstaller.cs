using DatabaseControllerProvider;
using FormPortal.Core.Models;
using Microsoft.AspNetCore.Identity;
using Mysqlx.Resultset;

namespace FormPortal.Installer.Core
{
    public static class DbInstaller
    {
        private static List<SqlTable> _tables = new();
        private static List<string> _data = new();
        static DbInstaller()
        {
            _tables.Add(new SqlTable("users", @"
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
);"));


            _tables.Add(new SqlTable("permissions", @"
CREATE TABLE permissions
(
	permission_id INTEGER NOT NULL AUTO_INCREMENT,
	identifier VARCHAR(50) NOT NULL,
	PRIMARY KEY (permission_id)
);
"));

            _tables.Add(new SqlTable("permission_description", @"
CREATE TABLE permission_description
(
	permission_id INTEGER NOT NULL,
	code VARCHAR(5) NOT NULL DEFAULT '',
	name VARCHAR(50) NOT NULL,
	description text NOT NULL,
	PRIMARY KEY(permission_id, code),
    FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("user_permissions", @"
CREATE TABLE user_permissions
(
	user_id INTEGER NOT NULL,
	permission_id INTEGER NOT NULL,
	PRIMARY KEY(user_id, permission_id),
	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_status", @"
CREATE TABLE form_status
(
	status_id INTEGER NOT NULL AUTO_INCREMENT,
	requires_approval TINYINT NOT NULL DEFAULT 0,
	is_completed TINYINT NOT NULL DEFAULT 0,
    sort_order INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(status_id)
);"));

            _tables.Add(new SqlTable("form_status_description", @"
CREATE TABLE form_status_description
(
	status_id INTEGER NOT NULL,
	code VARCHAR(5) NOT NULL,
	name VARCHAR(50) NOT NULL,
	description TEXT NOT NULL DEFAULT '',
	PRIMARY KEY (status_id, code),
	FOREIGN KEY (status_id) REFERENCES form_status(status_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("forms", @"
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
	PRIMARY KEY (form_id),
	FOREIGN KEY (default_status_id) REFERENCES form_status(status_id)
);"));

            _tables.Add(new SqlTable("form_to_user", @"
CREATE TABLE form_to_user
(
	form_id INTEGER NOT NULL,
	user_id INTEGER NOT NULL,
	PRIMARY KEY(form_id, user_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_managers", @"
CREATE TABLE form_managers
(
	form_id INTEGER NOT NULL,
	user_id INTEGER NOT NULL,
    receive_email TINYINT NOT NULL DEFAULT 0,
    can_approve TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY(form_id, user_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));


            _tables.Add(new SqlTable("form_rows", @"
CREATE TABLE form_rows
(
	row_id INTEGER NOT NULL AUTO_INCREMENT,
	form_id INTEGER NOT NULL,
	is_active TINYINT NOT NULL DEFAULT 0,
	rule_type VARCHAR(20) NOT NULL,
	sort_order INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY (row_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_columns", @"
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
);"));

            _tables.Add(new SqlTable("form_elements", @"
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
);"));

            _tables.Add(new SqlTable("form_elements_options", @"
CREATE TABLE form_elements_options
(
	element_option_id INTEGER NOT NULL AUTO_INCREMENT,
	element_id INTEGER NOT NULL,
	name VARCHAR(100) NOT NULL,
	PRIMARY KEY(element_option_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_checkbox_attributes", @"
CREATE TABLE form_elements_checkbox_attributes
(
	element_id INTEGER NOT NULL,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_date_attributes", @"
CREATE TABLE form_elements_date_attributes
(
	element_id INTEGER NOT NULL,
	is_current_date_default TINYINT NOT NULL DEFAULT 0,
	min_value date,
	max_value date,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_file_attributes", @"
CREATE TABLE form_elements_file_attributes
(
	element_id INTEGER NOT NULL,
	min_size INTEGER NOT NULL DEFAULT 0,
	max_size INTEGER NOT NULL DEFAULT 0,
	allow_multiple_files TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_file_types", @"
CREATE TABLE form_elements_file_types
(
	element_id INTEGER NOT NULL,
	content_type VARCHAR(255) NOT NULL,
	PRIMARY KEY(element_id, content_type),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_label_attributes", @"
CREATE TABLE form_elements_label_attributes
(
	element_id INTEGER NOT NULL,
	description TEXT NOT NULL,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_number_attributes", @"
CREATE TABLE form_elements_number_attributes
(
	element_id INTEGER NOT NULL,
	decimal_places INTEGER NOT NULL DEFAULT 0,
	min_value decimal NOT NULL DEFAULT 0,
	max_value decimal NOT NULL DEFAULT 0,
	is_summable TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);"));

            _tables.Add(new SqlTable("form_elements_radio_attributes", @"
CREATE TABLE form_elements_radio_attributes
(
	element_id INTEGER NOT NULL,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);
"));

            _tables.Add(new SqlTable("form_elements_select_attributes", @"
CREATE TABLE form_elements_select_attributes
(
	element_id INTEGER NOT NULL,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);
"));

            _tables.Add(new SqlTable("form_elements_table_attributes", @"
CREATE TABLE form_elements_table_attributes
(
	element_id INTEGER NOT NULL,
	allow_add_rows TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);
"));

            _tables.Add(new SqlTable("form_elements_textarea_attributes", @"
CREATE TABLE form_elements_textarea_attributes
(
	element_id INTEGER NOT NULL,
	regex_pattern text NOT NULL DEFAULT '',
	min_length INTEGER NOT NULL DEFAULT 0,
	max_length INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);
"));

            _tables.Add(new SqlTable("form_elements_text_attributes", @"
CREATE TABLE form_elements_text_attributes
(
	element_id INTEGER NOT NULL,
	regex_pattern text NOT NULL DEFAULT '',
	min_length INTEGER NOT NULL DEFAULT 0,
	max_length INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY(element_id),
	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
);
"));

            _tables.Add(new SqlTable("form_rules", @"
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
"));
            _tables.Add(new SqlTable("form_elements_number_calc_rules", @"
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
"));

            _tables.Add(new SqlTable("form_entries", @"
CREATE TABLE form_entries
(
	entry_id INTEGER NOT NULL AUTO_INCREMENT,
	form_id INTEGER NOT NULL,
	name VARCHAR(50) NOT NULL DEFAULT '',
	creation_date DATE NOT NULL DEFAULT CURRENT_DATE,
	creation_user_id INTEGER DEFAULT NULL,
	last_change DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	last_change_user_id INTEGER DEFAULT NULL,
	status_id INTEGER NOT NULL,
    approved TINYINT NOT NULL DEFAULT 0,
	PRIMARY KEY (entry_id),
	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (creation_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
	FOREIGN KEY (last_change_user_id) REFERENCES users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
	FOREIGN KEY (status_id) REFERENCES form_status(status_id)
);
"));

            _tables.Add(new SqlTable("form_entries_elements", @"
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
"));

            _tables.Add(new SqlTable("form_entries_table_elements", @"
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
"));

            _tables.Add(new SqlTable("form_entries_files", @"
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
"));

            _data.Add(@"
INSERT INTO permissions (permission_id, identifier) VALUES 
(1, 'EDIT_FORMS'),
(2, 'EDIT_ENTRIES'),
(3, 'EDIT_USERS'),
(4, 'DELETE_FORMS'),
(5, 'DELETE_ENTRIES'),
(6, 'APPROVE_FORM_ENTRIES'),
(7, 'EDIT_STATUS');");

            _data.Add(@"
INSERT INTO permission_description (permission_id, code, name, description) VALUES
(1, 'en', 'Form management','Allows the user the create, edit and delete new form templates.'),
(2, 'en', 'Submitted forms management','Allows the user the edit submitted form entries.'),
(3, 'en', 'User management','Allows the user to manage the users.'),
(4, 'en', 'Delete forms','Allows the user to entire forms.'),
(5, 'en', 'Delete Entries','Allows the user to delete submitted form entries.'),
(6, 'en', 'Approve forms','Allows the user to approve all form entries.'),
(7, 'en', 'Status management','Edit and create new statuses.'),
(1, 'de', 'Formularverwaltung','Anlegen und bearbeiten von Formularen.'),
(2, 'de', 'Formulareinträge ','Bearbeitung aller Formulareinträge.'),
(3, 'de', 'Benutzerverwaltung','Bearbeitung und Anlegen von Nutzern.'),
(4, 'de', 'Formulare löschen','Erlaubt es dem Benutzer Formulare zu löschen.'),
(5, 'de', 'Formulareinträge löschen','Erlaubt es dem Benutzer Formulareinträge zu löschen.'),
(6, 'de', 'Formulareinträge freigeben','Freigabe von allen Formulareinträgen.'),
(7, 'de', 'Status management','Erstellen und Bearbeiten von Stati.');");

            _data.Add(@"
INSERT INTO form_status (status_id, requires_approval, is_completed, sort_order) VALUES
(1, 0, 0, 1),
(2, 1, 0, 2),
(3, 0, 0, 3),
(4, 0, 1, 4);");

            _data.Add(@"
INSERT INTO form_status_description (status_id, code, name, description) VALUES
(1, 'en', 'Open', 'New and unprocessed form entries.'),
(2, 'en', 'Waiting for approval', 'Form entries that currently need to be approved.'),
(3, 'en', 'In process', 'Currently being processed by a form manager.'),
(4, 'en', 'Completed', 'Fully processed and completed form entries.'),
(1, 'de', 'Offen', 'Neue und unbearbeitete Formulareinträge.'),
(2, 'de', 'Warten auf Freigabe', 'Formulareinträge die derzeit noch freigegeben werden müssen.'),
(3, 'de', 'In Bearbeitung', 'Wird zurzeit durch einen Formularmanager bearbeitet.'),
(4, 'de', 'Erledigt', 'Vollständig bearbeitete und abgeschlossene Formulareinträge.');");
        }

        public static async Task InstallAsync(IDbController dbController)
        {
            int zähler = 0;
            Console.WriteLine("Creating tables...");
            foreach (var table in _tables)
            {
                Console.WriteLine($"[{table.TableName}] {++zähler}/{_tables.Count}");
                await dbController.QueryAsync(table.SQL);
            }
            Console.WriteLine("Tables have been successfully created!");
            Console.WriteLine("Inserting data...");
            zähler = 0;
            foreach (var sql in _data)
            {
                Console.WriteLine($"{++zähler}/{_data.Count}");
                await dbController.QueryAsync(sql);
            }
            Console.WriteLine("Data has been successfully inserted.");


        }

        public static string HashPassword(User user)
        {
            PasswordHasher<User> hasher = new();
            string passwordHashed = hasher.HashPassword(user, user.Password + user.Salt);

            return passwordHashed;
        }
    }
}
