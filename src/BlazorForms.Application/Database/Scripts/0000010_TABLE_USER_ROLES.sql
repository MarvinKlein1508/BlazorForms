CREATE TABLE user_roles 
(
	user_id int NOT NULL,
	role_id int NOT NULL,
	is_active BOOLEAN DEFAULT FALSE NOT NULL,
	CONSTRAINT pk_user_roles PRIMARY KEY (user_id, role_id),
	CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles(role_id),
	CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(user_id)
);