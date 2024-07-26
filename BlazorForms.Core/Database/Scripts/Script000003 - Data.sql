INSERT INTO languages (name, code, sort_order, status) VALUES 
('Deutsch', 'de', 0, 1),
('English', 'en', 1, 1);

INSERT INTO permissions (permission_id, identifier) VALUES 
(1, 'EDIT_FORMS'),
(2, 'EDIT_ENTRIES'),
(3, 'EDIT_USERS'),
(4, 'DELETE_FORMS'),
(5, 'DELETE_ENTRIES'),
(6, 'EDIT_STATUS');

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

INSERT INTO form_status (status_id, requires_approval, is_completed, sort_order) VALUES
(1, 0, 0, 1),
(2, 1, 0, 2),
(3, 0, 0, 3),
(4, 0, 1, 4);

INSERT INTO form_status_description (status_id, code, name, description) VALUES
(1, 'en', 'Open', 'New and unprocessed form entries.'),
(2, 'en', 'Waiting for approval', 'Form entries that currently need to be approved.'),
(3, 'en', 'In process', 'Currently being processed by a form manager.'),
(4, 'en', 'Completed', 'Fully processed and completed form entries.'),
(1, 'de', 'Offen', 'Neue und unbearbeitete Formulareinträge.'),
(2, 'de', 'Warten auf Freigabe', 'Formulareinträge die derzeit noch freigegeben werden müssen.'),
(3, 'de', 'In Bearbeitung', 'Wird zurzeit durch einen Formularmanager bearbeitet.'),
(4, 'de', 'Erledigt', 'Vollständig bearbeitete und abgeschlossene Formulareinträge.');