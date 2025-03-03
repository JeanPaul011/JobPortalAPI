CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "FullName" TEXT NOT NULL,
    "Role" TEXT NOT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "Companies" (
    "CompanyId" INTEGER NOT NULL CONSTRAINT "PK_Companies" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Industry" TEXT NOT NULL,
    "Location" TEXT NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Jobs" (
    "JobId" INTEGER NOT NULL CONSTRAINT "PK_Jobs" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Location" TEXT NOT NULL,
    "JobType" TEXT NOT NULL,
    "Salary" TEXT NOT NULL,
    "PostedOn" TEXT NOT NULL,
    "RecruiterId" TEXT NOT NULL,
    "CompanyId" INTEGER NOT NULL,
    CONSTRAINT "FK_Jobs_AspNetUsers_RecruiterId" FOREIGN KEY ("RecruiterId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Jobs_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("CompanyId") ON DELETE CASCADE
);

CREATE TABLE "Reviews" (
    "ReviewId" INTEGER NOT NULL CONSTRAINT "PK_Reviews" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "CompanyId" INTEGER NOT NULL,
    "Rating" INTEGER NOT NULL,
    "Comment" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    CONSTRAINT "FK_Reviews_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Reviews_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("CompanyId") ON DELETE CASCADE
);

CREATE TABLE "JobApplications" (
    "JobApplicationId" INTEGER NOT NULL CONSTRAINT "PK_JobApplications" PRIMARY KEY AUTOINCREMENT,
    "JobId" INTEGER NOT NULL,
    "JobSeekerId" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "AppliedOn" TEXT NOT NULL,
    CONSTRAINT "FK_JobApplications_AspNetUsers_JobSeekerId" FOREIGN KEY ("JobSeekerId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobApplications_Jobs_JobId" FOREIGN KEY ("JobId") REFERENCES "Jobs" ("JobId") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_JobApplications_JobId" ON "JobApplications" ("JobId");

CREATE INDEX "IX_JobApplications_JobSeekerId" ON "JobApplications" ("JobSeekerId");

CREATE INDEX "IX_Jobs_CompanyId" ON "Jobs" ("CompanyId");

CREATE INDEX "IX_Jobs_RecruiterId" ON "Jobs" ("RecruiterId");

CREATE INDEX "IX_Reviews_CompanyId" ON "Reviews" ("CompanyId");

CREATE INDEX "IX_Reviews_UserId" ON "Reviews" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250224221657_JobPortalSchema', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Reviews" RENAME COLUMN "ReviewId" TO "Id";

ALTER TABLE "Jobs" RENAME COLUMN "JobId" TO "Id";

ALTER TABLE "JobApplications" RENAME COLUMN "JobApplicationId" TO "Id";

ALTER TABLE "Companies" RENAME COLUMN "CompanyId" TO "Id";

CREATE TABLE "CompanyUser" (
    "CompaniesId" INTEGER NOT NULL,
    "RecruitersId" TEXT NOT NULL,
    CONSTRAINT "PK_CompanyUser" PRIMARY KEY ("CompaniesId", "RecruitersId"),
    CONSTRAINT "FK_CompanyUser_AspNetUsers_RecruitersId" FOREIGN KEY ("RecruitersId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyUser_Companies_CompaniesId" FOREIGN KEY ("CompaniesId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CompanyUser_RecruitersId" ON "CompanyUser" ("RecruitersId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250225151316_UpdatedModels', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "CompanyUser" RENAME TO "CompanyRecruiters";

DROP INDEX "IX_CompanyUser_RecruitersId";

CREATE INDEX "IX_CompanyRecruiters_RecruitersId" ON "CompanyRecruiters" ("RecruitersId");

CREATE TABLE "ef_temp_CompanyRecruiters" (
    "CompaniesId" INTEGER NOT NULL,
    "RecruitersId" TEXT NOT NULL,
    CONSTRAINT "PK_CompanyRecruiters" PRIMARY KEY ("CompaniesId", "RecruitersId"),
    CONSTRAINT "FK_CompanyRecruiters_AspNetUsers_RecruitersId" FOREIGN KEY ("RecruitersId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyRecruiters_Companies_CompaniesId" FOREIGN KEY ("CompaniesId") REFERENCES "Companies" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_CompanyRecruiters" ("CompaniesId", "RecruitersId")
SELECT "CompaniesId", "RecruitersId"
FROM "CompanyRecruiters";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "CompanyRecruiters";

ALTER TABLE "ef_temp_CompanyRecruiters" RENAME TO "CompanyRecruiters";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_CompanyRecruiters_RecruitersId" ON "CompanyRecruiters" ("RecruitersId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250227002528_AddRecruitersToCompanies', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "AspNetUsers" ADD "RefreshToken" TEXT NULL;

ALTER TABLE "AspNetUsers" ADD "RefreshTokenExpiry" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250302190136_AddRefreshTokenToUser', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250302191716_AddRefreshToken', '8.0.0');

COMMIT;

