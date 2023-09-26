CREATE DATABASE IF NOT EXISTS realchat;

USE realchat;

CREATE TABLE organization (
    id VARCHAR(36) NOT NULL,
    display_name NVARCHAR(50) NOT NULL,
    created_time DATETIME,
    PRIMARY KEY (id)
);

CREATE TABLE chatbot (
    id VARCHAR(36) NOT NULL,
    display_name NVARCHAR(50) NOT NULL,
    created_time DATETIME,
    organization_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_chatbot_organization` 
        FOREIGN KEY (organization_id) REFERENCES organization (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT
);

CREATE TABLE knowledge_base (
    id VARCHAR(36) NOT NULL,
    created_time DATETIME NOT NULL,
    name NVARCHAR(50) NOT NULL,
    chatbot_id VARCHAR(36) NOT NULL,
    organization_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_knowledge_base_chatbot`
        FOREIGN KEY (chatbot_id) REFERENCES chatbot (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT,
    CONSTRAINT `fk_knowledge_base_organization` 
        FOREIGN KEY (organization_id) REFERENCES organization (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT
);

CREATE TABLE information_chunk (
    id VARCHAR(36) NOT NULL,
    created_time DATETIME NOT NULL,
    content TEXT NOT NULL,
    chunk_number INT NOT NULL,
    knowledge_base_id VARCHAR(36) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT `fk_information_chunk_knowledge_base`
        FOREIGN KEY (knowledge_base_id) REFERENCES knowledge_base (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT
);

CREATE TABLE script_type (
    id INT NOT NULL,
    type VARCHAR(50) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE script (
    id VARCHAR(36) NOT NULL,
    trigger_text NVARCHAR(100) NOT NULL,
    action TEXT NOT NULL,
    chatbot_id VARCHAR(36) NOT NULL,
    organization_id VARCHAR(36) NOT NULL,
    script_type_id INT NOT NULL,
    created_time DATETIME,
    PRIMARY KEY (id),
    CONSTRAINT `fk_script_chatbot`
        FOREIGN KEY (chatbot_id) REFERENCES chatbot (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT,
    CONSTRAINT `fk_script_script_type` 
        FOREIGN KEY (script_type_id) REFERENCES script_type (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT,
    CONSTRAINT `fk_script_organization` 
        FOREIGN KEY (organization_id) REFERENCES organization (id)
        ON DELETE CASCADE
        ON UPDATE RESTRICT
);

INSERT INTO script_type VALUES (1, 'RESPONSE_TEXT_IF_PROMPT_MATCHES');

-- CREATE TABLE user (
--     id VARCHAR(36) NOT NULL,
--     username VARCHAR(25) NOT NULL,
--     email VARCHAR(125) NOT NULL,
--     password_hash VARCHAR(32),
--     created_time DATETIME NOT NULL
--     PRIMARY KEY (id)
-- );