CREATE SCHEMA IF NOT EXISTS identity;

CREATE TABLE identity.users
(
    id                          uuid         NOT NULL,
    email                       text         NOT NULL,
    external_identity_provider  varchar(50)  NOT NULL,
    external_identity_subject   varchar(255) NOT NULL,
    status                      varchar(20)  NOT NULL,
    is_email_verified           boolean      NOT NULL DEFAULT false,
    created_at_utc              timestamptz  NOT NULL,
    email_verified_at_utc       timestamptz  NULL,
    suspended_at_utc            timestamptz  NULL,
    deactivated_at_utc          timestamptz  NULL,
    suspension_reason           varchar(500) NULL,

    CONSTRAINT pk_users PRIMARY KEY (id),

    CONSTRAINT uq_users_email UNIQUE (email),

    CONSTRAINT uq_users_external_identity
        UNIQUE (external_identity_provider, external_identity_subject),

    CONSTRAINT ck_users_email_normalized CHECK (email = lower(trim(email))),

    CONSTRAINT ck_users_email_not_blank CHECK (length(email) BETWEEN 3 AND 254),

    CONSTRAINT ck_users_external_identity_provider_normalized
        CHECK (
            external_identity_provider = lower(trim(external_identity_provider))
            AND length(external_identity_provider) BETWEEN 1 AND 50
        ),

    CONSTRAINT ck_users_external_identity_subject_not_blank
        CHECK (
            external_identity_subject = trim(external_identity_subject)
            AND length(external_identity_subject) BETWEEN 1 AND 255
        ),

    CONSTRAINT ck_users_status
        CHECK (
            status IN (
                'PendingActivation',
                'Active',
                'Suspended',
                'Deactivated'
            )
        ),

    CONSTRAINT ck_users_email_verification
        CHECK (
            (is_email_verified = true AND email_verified_at_utc IS NOT NULL)
            OR
            (is_email_verified = false AND email_verified_at_utc IS NULL)
        ),

    CONSTRAINT ck_users_suspension
        CHECK (
            (
                status = 'Suspended'
                AND suspended_at_utc IS NOT NULL
                AND suspension_reason IS NOT NULL
                AND length(trim(suspension_reason)) BETWEEN 1 AND 500
            )
            OR
            (
                status <> 'Suspended'
                AND suspended_at_utc IS NULL
                AND suspension_reason IS NULL
            )
        ),

    CONSTRAINT ck_users_deactivation
        CHECK (
            (status = 'Deactivated' AND deactivated_at_utc IS NOT NULL)
            OR
            (status <> 'Deactivated' AND deactivated_at_utc IS NULL)
        ),

    CONSTRAINT ck_users_email_verified_after_creation
        CHECK (
            email_verified_at_utc IS NULL
            OR email_verified_at_utc >= created_at_utc
        ),

    CONSTRAINT ck_users_suspended_after_creation
        CHECK (
            suspended_at_utc IS NULL
            OR suspended_at_utc >= created_at_utc
        ),

    CONSTRAINT ck_users_deactivated_after_creation
        CHECK (
            deactivated_at_utc IS NULL
            OR deactivated_at_utc >= created_at_utc
        )
);

COMMENT ON TABLE identity.users IS
    'Accounts and external authentication identities.';

COMMENT ON COLUMN identity.users.email IS
    'Normalized lowercase email address.';

COMMENT ON COLUMN identity.users.external_identity_provider IS
    'Normalized authentication provider name, for example keycloak.';

COMMENT ON COLUMN identity.users.external_identity_subject IS
    'Stable subject identifier supplied by the authentication provider.';