CREATE SCHEMA IF NOT EXISTS social;

CREATE TABLE social.user_profiles
(
    id              uuid         NOT NULL,
    user_id         uuid         NOT NULL,
    username        text         NOT NULL,
    display_name    varchar(80)  NOT NULL,
    biography       varchar(500) NOT NULL DEFAULT '',
    avatar_url      text         NULL,
    visibility      varchar(20)  NOT NULL,
    created_at_utc  timestamptz  NOT NULL,
    updated_at_utc  timestamptz  NOT NULL,

    CONSTRAINT pk_user_profiles PRIMARY KEY (id),
    CONSTRAINT uq_user_profiles_user_id UNIQUE (user_id),
    CONSTRAINT uq_user_profiles_username UNIQUE (username),
    CONSTRAINT ck_user_profiles_username_normalized
        CHECK (username = lower(username)),
    CONSTRAINT ck_user_profiles_username_length
        CHECK (char_length(username) BETWEEN 3 AND 30),
    CONSTRAINT ck_user_profiles_display_name_length
        CHECK (char_length(trim(display_name)) BETWEEN 1 AND 80),
    CONSTRAINT ck_user_profiles_biography_length
        CHECK (char_length(biography) <= 500),
    CONSTRAINT ck_user_profiles_visibility
        CHECK (visibility IN ('Public', 'FollowersOnly', 'Private')),
    CONSTRAINT ck_user_profiles_timestamps
        CHECK (updated_at_utc >= created_at_utc)
);
