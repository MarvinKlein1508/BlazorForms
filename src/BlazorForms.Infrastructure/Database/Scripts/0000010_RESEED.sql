SELECT SETVAL(pg_get_serial_sequence('form_status', 'status_id'), (SELECT COALESCE(MAX(status_id), 0) FROM form_status) + 1, false);
SELECT SETVAL(pg_get_serial_sequence('user_groups', 'user_group_id'), (SELECT COALESCE(MAX(user_group_id), 0) FROM user_groups) + 1, false);
SELECT SETVAL(pg_get_serial_sequence('languages', 'language_id'), (SELECT COALESCE(MAX(language_id), 0) FROM languages) + 1, false);
