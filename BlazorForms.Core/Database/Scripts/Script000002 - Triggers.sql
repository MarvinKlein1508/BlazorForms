DELIMITER //

CREATE TRIGGER update_status_id
    AFTER INSERT
    ON form_entry_history
    FOR EACH ROW
BEGIN
    UPDATE form_entries SET 
        status_id = NEW.status_id,
        last_change_user_id = NEW.user_id,
        last_change = CURRENT_TIMESTAMP
    WHERE entry_id = NEW.entry_id;
END;
//

DELIMITER ;
