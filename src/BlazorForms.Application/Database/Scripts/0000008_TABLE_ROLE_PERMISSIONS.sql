CREATE TABLE role_permissions
(
	role_id INTEGER NOT NULL,
	permission_id INTEGER NOT NULL,
	is_active BOOLEAN NOT NULL DEFAULT FALSE,
	CONSTRAINT pk_role_permissions PRIMARY KEY(role_id, permission_id),
	CONSTRAINT fk_role_permissions_role FOREIGN KEY (role_id) REFERENCES roles(role_id),
	CONSTRAINT fk_role_permissions_permission FOREIGN KEY (permission_id) REFERENCES permissions(permission_id)
);