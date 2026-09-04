CREATE SCHEMA IF NOT EXISTS social;

CREATE TABLE IF NOT EXISTS social.user_profiles (
    id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    username varchar(30) NOT NULL,
    display_name varchar(80) NOT NULL,
    biography varchar(500) NOT NULL DEFAULT '',
    avatar_url text NULL,
    visibility varchar(20) NOT NULL,
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL,
    CONSTRAINT uq_user_profiles_user_id UNIQUE (user_id),
    CONSTRAINT uq_user_profiles_username UNIQUE (username)
);
