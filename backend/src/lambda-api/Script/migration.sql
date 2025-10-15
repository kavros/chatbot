CREATE TABLE
IF NOT EXISTS "__EFMigrationsHistory"
(
    "MigrationId" character varying
(150) NOT NULL,
    "ProductVersion" character varying
(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY
("MigrationId")
);

CREATE TABLE
IF NOT EXISTS owner
(
    id uuid NOT NULL,
    name text NOT NULL,
    city text NOT NULL,
    telephone text NOT NULL,
    CONSTRAINT "PK_owner" PRIMARY KEY
(id)
);

INSERT INTO "__EFMigrationsHistory"
    ("MigrationId", "ProductVersion")
VALUES
    ('20250730192520_initial', '9.0.7')
ON CONFLICT
("MigrationId") DO NOTHING;

CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "RoleId" uuid NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "AspNetRoles" (
    "Id" uuid NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "UserId" uuid NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" uuid NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey")
);

CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId")
);

CREATE TABLE IF NOT EXISTS "AspNetUsers" (
    "Id" uuid NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (
    "UserId" uuid NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

CREATE UNIQUE INDEX async IF NOT EXISTS "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX async IF NOT EXISTS "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX async IF NOT EXISTS "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");