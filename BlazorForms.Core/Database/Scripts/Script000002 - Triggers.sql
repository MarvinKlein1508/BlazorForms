CREATE TRIGGER update_status_id
    AFTER INSERT
    ON form_entry_history FOR EACH ROW
BEGIN
    UPDATE form_entries SET 
        status_id = new.status_id,
        last_change_user_id = new.user_id,
        last_change = CURRENT_TIMESTAMP
    WHERE entry_id = new.entry_id;
END; 