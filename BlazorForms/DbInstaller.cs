using BlazorForms.Core.Models;
using DbController;
using Microsoft.AspNetCore.Identity;
using Spectre.Console;

namespace BlazorForms
{
    public static class DbInstaller
    {


        public static async Task InstallAsync(IDbController dbController)
        {

            await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                
                var task1 = ctx.AddTask("[green]Create tables[/]");

                var tables = GetTables().ToArray();
                task1.MaxValue = tables.Length;

                int counter = 1;
                while (!ctx.IsFinished)
                {
                    foreach (var table in tables)
                    {
                        await dbController.QueryAsync(table.SQL);
                        task1.Value = counter++;
                    }
                }

                counter = 1;

                var triggers = GetTriggers().ToArray();

                var task2 = ctx.AddTask("[green]Create triggers[/]");
                task2.MaxValue = triggers.Length;
                while (!ctx.IsFinished)
                {
                    foreach (var sql in triggers)
                    {
                        await dbController.QueryAsync(sql);
                        task2.Value = counter++;
                    }
                }

                counter = 1;

                var defaultData = GetDefaultData().ToArray();

                var task3 = ctx.AddTask("[green]Create default data[/]");
                task3.MaxValue = defaultData.Length;
                while (!ctx.IsFinished)
                {
                    foreach (var sql in defaultData)
                    { 
                        await dbController.QueryAsync(sql);
                        task3.Value = counter++;
                    }
                }
            });
        }

        private static IEnumerable<SqlTable> GetTables()
        {
            yield return new SqlTable
            {
                TableName = "languages",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "users",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "permissions",
                SQL =
                """
                CREATE TABLE permissions
                (
                	permission_id INTEGER NOT NULL AUTO_INCREMENT,
                	identifier VARCHAR(50) NOT NULL,
                	PRIMARY KEY (permission_id)
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "permission_description",
                SQL =
                """
                CREATE TABLE permission_description
                (
                	permission_id INTEGER NOT NULL,
                	code VARCHAR(5) NOT NULL DEFAULT '',
                	name VARCHAR(50) NOT NULL,
                	description text NOT NULL,
                	PRIMARY KEY(permission_id, code),
                	FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };


            yield return new SqlTable
            {
                TableName = "user_permissions",
                SQL =
                """
                CREATE TABLE user_permissions
                (
                	user_id INTEGER NOT NULL,
                	permission_id INTEGER NOT NULL,
                	PRIMARY KEY(user_id, permission_id),
                	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (permission_id) REFERENCES permissions(permission_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "user_filter",
                SQL =
                """
                CREATE TABLE user_filter
                (
                	user_id INTEGER NOT NULL,
                	filter_type VARCHAR(255) NOT NULL,
                	page VARCHAR(255) NOT NULL,
                	serialized longtext NOT NULL CHECK (json_valid(serialized)),
                	PRIMARY KEY (user_id, filter_type, page),
                	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_status",
                SQL =
                """
                CREATE TABLE form_status
                (
                	status_id INTEGER NOT NULL AUTO_INCREMENT,
                	requires_approval TINYINT NOT NULL DEFAULT 0,
                	is_completed TINYINT NOT NULL DEFAULT 0,
                	sort_order INTEGER NOT NULL DEFAULT 0,
                	PRIMARY KEY(status_id)
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_status_description",
                SQL =
                """
                CREATE TABLE form_status_description
                (
                	status_id INTEGER NOT NULL,
                	code VARCHAR(5) NOT NULL,
                	name VARCHAR(50) NOT NULL,
                	description varchar(255) NOT NULL DEFAULT '',
                	PRIMARY KEY (status_id, code),
                	FOREIGN KEY (status_id) REFERENCES form_status(status_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };


            yield return new SqlTable
            {
                TableName = "forms",
                SQL =
                """
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
                	PRIMARY KEY (form_id),
                	FOREIGN KEY (default_status_id) REFERENCES form_status(status_id),
                	FOREIGN KEY (language_id) REFERENCES languages(language_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_to_user",
                SQL =
                """
                CREATE TABLE form_to_user
                (
                	form_id INTEGER NOT NULL,
                	user_id INTEGER NOT NULL,
                	PRIMARY KEY(form_id, user_id),
                	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_managers",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_rows",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_columns",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_options",
                SQL =
                """
                CREATE TABLE form_elements_options
                (
                	element_option_id INTEGER NOT NULL AUTO_INCREMENT,
                	element_id INTEGER NOT NULL,
                	name VARCHAR(100) NOT NULL,
                	PRIMARY KEY(element_option_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_checkbox_attributes",
                SQL =
                """
                CREATE TABLE form_elements_checkbox_attributes
                (
                	element_id INTEGER NOT NULL,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };


            yield return new SqlTable
            {
                TableName = "form_elements_date_attributes",
                SQL =
                """
                CREATE TABLE form_elements_date_attributes
                (
                	element_id INTEGER NOT NULL,
                	is_current_date_default TINYINT NOT NULL DEFAULT 0,
                	min_value date,
                	max_value date,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_file_attributes",
                SQL =
                """
                CREATE TABLE form_elements_file_attributes
                (
                	element_id INTEGER NOT NULL,
                	min_size INTEGER NOT NULL DEFAULT 0,
                	max_size INTEGER NOT NULL DEFAULT 0,
                	allow_multiple_files TINYINT NOT NULL DEFAULT 0,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_file_types",
                SQL =
                """
                CREATE TABLE form_elements_file_types
                (
                	element_id INTEGER NOT NULL,
                	content_type VARCHAR(255) NOT NULL,
                	PRIMARY KEY(element_id, content_type),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_label_attributes",
                SQL =
                """
                CREATE TABLE form_elements_label_attributes
                (
                	element_id INTEGER NOT NULL,
                	description TEXT NOT NULL,
                	show_on_pdf TINYINT NOT NULL DEFAULT 1,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_number_attributes",
                SQL =
                """
                CREATE TABLE form_elements_number_attributes
                (
                	element_id INTEGER NOT NULL,
                	decimal_places INTEGER NOT NULL DEFAULT 0,
                	min_value decimal NOT NULL DEFAULT 0,
                	max_value decimal NOT NULL DEFAULT 0,
                	is_summable TINYINT NOT NULL DEFAULT 0,
                    default_value decimal NOT NULL DEFAULT 0,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_radio_attributes",
                SQL =
                """
                CREATE TABLE form_elements_radio_attributes
                (
                	element_id INTEGER NOT NULL,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_select_attributes",
                SQL =
                """
                CREATE TABLE form_elements_select_attributes
                (
                	element_id INTEGER NOT NULL,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_table_attributes",
                SQL =
                """
                CREATE TABLE form_elements_table_attributes
                (
                	element_id INTEGER NOT NULL,
                	allow_add_rows TINYINT NOT NULL DEFAULT 0,
                	PRIMARY KEY(element_id),
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_textarea_attributes",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_text_attributes",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_rules",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_elements_number_calc_rules",
                SQL =
                """
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
                """
            };

            yield return new SqlTable
            {
                TableName = "form_entries",
                SQL =
                """
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
                """
            };


            yield return new SqlTable
            {
                TableName = "form_entries_elements",
                SQL =
                """
                CREATE TABLE form_entries_elements
                (
                	entry_id INTEGER NOT NULL,
                	form_id INTEGER NOT NULL,
                	element_id INTEGER NOT NULL,
                	value_boolean TINYINT NOT NULL DEFAULT 0,
                	value_string TEXT,
                	value_number DECIMAL(10,5) NOT NULL DEFAULT 0,
                	value_date DATE DEFAULT NULL,
                	PRIMARY KEY (entry_id, form_id, element_id),
                	FOREIGN KEY (form_id) REFERENCES forms(form_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };


            yield return new SqlTable
            {
                TableName = "form_entries_table_elements",
                SQL =
                """
                CREATE TABLE form_entries_table_elements
                (
                	table_row_number INTEGER NOT NULL,
                	table_parent_element_id INTEGER NOT NULL,
                	entry_id INTEGER NOT NULL,
                	element_id INTEGER NOT NULL,
                	value_boolean TINYINT NOT NULL DEFAULT 0,
                	value_string TEXT,
                	value_number DECIMAL(10,5) NOT NULL DEFAULT 0,
                	value_date DATE DEFAULT NULL,
                	PRIMARY KEY (table_row_number, table_parent_element_id, entry_id, element_id),
                	FOREIGN KEY (entry_id) REFERENCES form_entries(entry_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (element_id) REFERENCES form_elements(element_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };


            yield return new SqlTable
            {
                TableName = "form_entries_files",
                SQL =
                """
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
                """
            };


            yield return new SqlTable
            {
                TableName = "form_entry_history",
                SQL =
                """
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
                """
            };


            yield return new SqlTable
            {
                TableName = "form_entry_history_notify",
                SQL =
                """
                CREATE TABLE form_entry_history_notify 
                (
                	history_id INTEGER NOT NULL,
                	user_id INTEGER NOT NULL,
                	notify TINYINT NOT NULL DEFAULT 0,
                	PRIMARY KEY (history_id, user_id),
                	FOREIGN KEY (history_id) REFERENCES form_entry_history(history_id) ON DELETE CASCADE ON UPDATE CASCADE,
                	FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
                );
                """
            };

            yield return new SqlTable
            { 
                TableName = "notifications",
                SQL =
                """
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
                """
            };
        }
        private static IEnumerable<string> GetTriggers()
        {
            yield return
                """
                CREATE TRIGGER update_status_id
                    AFTER INSERT
                    ON form_entry_history FOR EACH ROW
                BEGIN
                    UPDATE form_entries SET status_id = new.status_id WHERE entry_id = new.entry_id;
                END;   
                """;
        }
        private static IEnumerable<string> GetDefaultData()
        {
            yield return
                """
                INSERT INTO languages (name, code, sort_order, status) VALUES 
                ('Deutsch', 'de', 0, 1),
                ('English', 'en', 1, 1);
                """;

            yield return
                """
                INSERT INTO permissions (permission_id, identifier) VALUES 
                (1, 'EDIT_FORMS'),
                (2, 'EDIT_ENTRIES'),
                (3, 'EDIT_USERS'),
                (4, 'DELETE_FORMS'),
                (5, 'DELETE_ENTRIES'),
                (6, 'EDIT_STATUS');
                """;

            yield return
                """
                INSERT INTO permission_description (permission_id, code, name, description) VALUES
                (1, 'en', 'Form management','Allows the user the create, edit and delete new form templates.'),
                (2, 'en', 'Submitted forms management','Allows the user the edit submitted form entries.'),
                (3, 'en', 'User management','Allows the user to manage the users.'),
                (4, 'en', 'Delete forms','Allows the user to entire forms.'),
                (5, 'en', 'Delete Entries','Allows the user to delete submitted form entries.'),
                (6, 'en', 'Status management','Edit and create new statuses.'),
                (1, 'de', 'Formularverwaltung','Anlegen und bearbeiten von Formularen.'),
                (2, 'de', 'Formulareinträge ','Bearbeitung aller Formulareinträge.'),
                (3, 'de', 'Benutzerverwaltung','Bearbeitung und Anlegen von Nutzern.'),
                (4, 'de', 'Formulare löschen','Erlaubt es dem Benutzer Formulare zu löschen.'),
                (5, 'de', 'Formulareinträge löschen','Erlaubt es dem Benutzer Formulareinträge zu löschen.'),
                (6, 'de', 'Statusverwaltung','Erstellen und Bearbeiten von Stati.');
                """;

            yield return
                """
                INSERT INTO form_status (status_id, requires_approval, is_completed, sort_order) VALUES
                (1, 0, 0, 1),
                (2, 1, 0, 2),
                (3, 0, 0, 3),
                (4, 0, 1, 4);
                """;

            yield return
                """
                INSERT INTO form_status_description (status_id, code, name, description) VALUES
                (1, 'en', 'Open', 'New and unprocessed form entries.'),
                (2, 'en', 'Waiting for approval', 'Form entries that currently need to be approved.'),
                (3, 'en', 'In process', 'Currently being processed by a form manager.'),
                (4, 'en', 'Completed', 'Fully processed and completed form entries.'),
                (1, 'de', 'Offen', 'Neue und unbearbeitete Formulareinträge.'),
                (2, 'de', 'Warten auf Freigabe', 'Formulareinträge die derzeit noch freigegeben werden müssen.'),
                (3, 'de', 'In Bearbeitung', 'Wird zurzeit durch einen Formularmanager bearbeitet.'),
                (4, 'de', 'Erledigt', 'Vollständig bearbeitete und abgeschlossene Formulareinträge.');
                """;

        }
        public static string HashPassword(User user)
        {
            PasswordHasher<User> hasher = new();
            string passwordHashed = hasher.HashPassword(user, user.Password + user.Salt);

            return passwordHashed;
        }

        private class SqlTable
        {
            public required string TableName { get; set; }
            public required string SQL { get; set; }
        }
    }
}
