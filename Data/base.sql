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