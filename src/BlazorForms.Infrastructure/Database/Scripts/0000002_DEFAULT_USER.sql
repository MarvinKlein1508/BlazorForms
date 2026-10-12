INSERT INTO users (
    username,
    role,
    display_name,
    email,
    password,
    salt,
    account_type,
    is_active,
    two_factor_enabled,
    lockout_enabled
)
VALUES (
    'admin',
    3,
    'Admin',
    'admin@localhost',
    'AQAAAAIAAYagAAAAEOpXMr79G25colQMJ6NiO2B1jEsAAqxgaMAO6FbGCMrBw+zhj6/t+dd37as3Dr9ecw==',
    'cCGqI5ntbBbqamEwerd13P2kBmKSLY8oxxWHH29tkjo=',
    1,
    TRUE,
    FALSE,
    FALSE
);
