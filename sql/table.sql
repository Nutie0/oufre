drop table users;

CREATE TABLE users(
    user_id SERIAL PRIMARY KEY,       
    email VARCHAR(55) NOT NULL,
    username VARCHAR(55) NOT NULL UNIQUE,   
    password TEXT NOT NULL,
    isMailConfirmed BOOLEAN DEFAULT FALSE, 
    tokens TEXT NOT NULL,
    created TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);




CREATE OR REPLACE FUNCTION SP_users(
    IN p_user_id INT,
    IN p_email VARCHAR,
    IN p_username VARCHAR,
    IN p_password TEXT,
    IN p_tokens TEXT,
    IN p_OperationType INT
)
RETURNS TABLE (
    user_id INT,
    email VARCHAR,
    username VARCHAR,
    password TEXT,
    isMailConfirmed BOOLEAN,
    tokens TEXT,
    Created TIMESTAMP,
    Updated TIMESTAMP
) AS $$
BEGIN
    IF p_OperationType = 1 THEN
        -- Vérifier si l'utilisateur existe déjà
        IF EXISTS (
            SELECT 1 
            FROM users li 
            WHERE li.email = p_email OR li.username = p_username
        ) THEN
            RAISE EXCEPTION 'User with this emailId or username already exists.';
        END IF;

        -- Inscription (Signup)
        RETURN QUERY
        INSERT INTO users (email, username, password, isMailConfirmed, tokens, Created, Updated)
        VALUES (p_email, p_username, p_password, FALSE, p_tokens, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
        RETURNING 
            users.user_id,  -- Qualification explicite de la colonne
            users.email,
            users.username,
            users.password,
            users.isMailConfirmed,
            users.tokens,
            users.Created,
            users.Updated;

    ELSIF p_OperationType = 2 THEN
        -- Confirmation de l'email (Confirm Mail)
        IF p_username IS NULL OR p_username = '' THEN
            RAISE EXCEPTION 'Invalid user. Please create account.';
        END IF;

        -- Mise à jour de la confirmation de l'email
        RETURN QUERY
        UPDATE users
        SET isMailConfirmed = TRUE, Updated = CURRENT_TIMESTAMP
        WHERE users.username = p_username AND users.tokens = p_tokens
        RETURNING 
            users.user_id,  -- Qualification explicite de la colonne
            users.email,
            users.username,
            users.password,
            users.isMailConfirmed,
            users.tokens,
            users.Created,
            users.Updated;

    ELSE
        RAISE EXCEPTION 'Invalid OperationType. Use 1 for Signup, 2 for Confirm Mail.';
    END IF;
END;
$$ LANGUAGE plpgsql;




    SELECT * FROM SP_users(
        NULL,
        'titi@yahoo.com',
        'titi',
        'hashedpassword123',
        'tokensGenerated',
        1
    );


    SELECT * FROM SP_users(
        NULL,
        NULL,
        'titi',
        NULL,
        'tokensGenerated',
        2
    );

