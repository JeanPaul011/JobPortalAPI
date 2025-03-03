--
-- File generated with SQLiteStudio v3.4.17 on Mon Mar 3 05:10:27 2025
--
-- Text encoding used: UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: __EFMigrationsHistory
DROP TABLE IF EXISTS __EFMigrationsHistory;

CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId    TEXT NOT NULL
                        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
    ProductVersion TEXT NOT NULL
);

INSERT INTO __EFMigrationsHistory (
                                      MigrationId,
                                      ProductVersion
                                  )
                                  VALUES (
                                      '20250224221657_JobPortalSchema',
                                      '8.0.0'
                                  );

INSERT INTO __EFMigrationsHistory (
                                      MigrationId,
                                      ProductVersion
                                  )
                                  VALUES (
                                      '20250225151316_UpdatedModels',
                                      '8.0.0'
                                  );

INSERT INTO __EFMigrationsHistory (
                                      MigrationId,
                                      ProductVersion
                                  )
                                  VALUES (
                                      '20250227002528_AddRecruitersToCompanies',
                                      '8.0.0'
                                  );

INSERT INTO __EFMigrationsHistory (
                                      MigrationId,
                                      ProductVersion
                                  )
                                  VALUES (
                                      '20250302190136_AddRefreshTokenToUser',
                                      '8.0.0'
                                  );

INSERT INTO __EFMigrationsHistory (
                                      MigrationId,
                                      ProductVersion
                                  )
                                  VALUES (
                                      '20250302191716_AddRefreshToken',
                                      '8.0.0'
                                  );


-- Table: AspNetRoleClaims
DROP TABLE IF EXISTS AspNetRoleClaims;

CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id         INTEGER NOT NULL
                       CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY AUTOINCREMENT,
    RoleId     TEXT    NOT NULL,
    ClaimType  TEXT    NULL,
    ClaimValue TEXT    NULL,
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (
        RoleId
    )
    REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);


-- Table: AspNetRoles
DROP TABLE IF EXISTS AspNetRoles;

CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id               TEXT NOT NULL
                          CONSTRAINT PK_AspNetRoles PRIMARY KEY,
    Name             TEXT NULL,
    NormalizedName   TEXT NULL,
    ConcurrencyStamp TEXT NULL
);

INSERT INTO AspNetRoles (
                            Id,
                            Name,
                            NormalizedName,
                            ConcurrencyStamp
                        )
                        VALUES (
                            'dca525d2-1269-41fc-a866-d6ce1a68aeee',
                            'Admin',
                            'ADMIN',
                            NULL
                        );

INSERT INTO AspNetRoles (
                            Id,
                            Name,
                            NormalizedName,
                            ConcurrencyStamp
                        )
                        VALUES (
                            '30a17e25-bc82-4336-ab5a-860fbc9923e5',
                            'Recruiter',
                            'RECRUITER',
                            NULL
                        );

INSERT INTO AspNetRoles (
                            Id,
                            Name,
                            NormalizedName,
                            ConcurrencyStamp
                        )
                        VALUES (
                            '0205f0f5-cd69-4295-80e1-2a32e0dd0e95',
                            'JobSeeker',
                            'JOBSEEKER',
                            NULL
                        );


-- Table: AspNetUserClaims
DROP TABLE IF EXISTS AspNetUserClaims;

CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id         INTEGER NOT NULL
                       CONSTRAINT PK_AspNetUserClaims PRIMARY KEY AUTOINCREMENT,
    UserId     TEXT    NOT NULL,
    ClaimType  TEXT    NULL,
    ClaimValue TEXT    NULL,
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (
        UserId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);


-- Table: AspNetUserLogins
DROP TABLE IF EXISTS AspNetUserLogins;

CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider       TEXT NOT NULL,
    ProviderKey         TEXT NOT NULL,
    ProviderDisplayName TEXT NULL,
    UserId              TEXT NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (
        LoginProvider,
        ProviderKey
    ),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (
        UserId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);


-- Table: AspNetUserRoles
DROP TABLE IF EXISTS AspNetUserRoles;

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId TEXT NOT NULL,
    RoleId TEXT NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (
        UserId,
        RoleId
    ),
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (
        RoleId
    )
    REFERENCES AspNetRoles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (
        UserId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                '08269f59-e898-432b-8e6d-a1cb31b86f25',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                '11843320-34ec-44f8-b848-d6b76bbcb7d0',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                'faceb3a8-a10d-416b-aa4d-b8fa1d88fdb2',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                'c7ab6bd4-573b-4a6b-a083-2a6ceb701353',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                'b0a1d7e4-ff21-47ed-8205-2fd6f2e3a742',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                '6ae6973a-c59d-478e-8cdc-5d879512f167',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );

INSERT INTO AspNetUserRoles (
                                UserId,
                                RoleId
                            )
                            VALUES (
                                '592d81aa-153c-46dc-8208-a45d7a89c74e',
                                'dca525d2-1269-41fc-a866-d6ce1a68aeee'
                            );


-- Table: AspNetUsers
DROP TABLE IF EXISTS AspNetUsers;

CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id                   TEXT    NOT NULL
                                 CONSTRAINT PK_AspNetUsers PRIMARY KEY,
    FullName             TEXT    NOT NULL,
    Role                 TEXT    NOT NULL,
    UserName             TEXT    NULL,
    NormalizedUserName   TEXT    NULL,
    Email                TEXT    NULL,
    NormalizedEmail      TEXT    NULL,
    EmailConfirmed       INTEGER NOT NULL,
    PasswordHash         TEXT    NULL,
    SecurityStamp        TEXT    NULL,
    ConcurrencyStamp     TEXT    NULL,
    PhoneNumber          TEXT    NULL,
    PhoneNumberConfirmed INTEGER NOT NULL,
    TwoFactorEnabled     INTEGER NOT NULL,
    LockoutEnd           TEXT    NULL,
    LockoutEnabled       INTEGER NOT NULL,
    AccessFailedCount    INTEGER NOT NULL,
    RefreshToken         TEXT    NULL,
    RefreshTokenExpiry   TEXT    NULL
);

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            '08269f59-e898-432b-8e6d-a1cb31b86f25',
                            'Super Admin',
                            'Admin',
                            'admin_super99@secure.com',
                            'ADMIN_SUPER99@SECURE.COM',
                            'admin_super99@secure.com',
                            'ADMIN_SUPER99@SECURE.COM',
                            0,
                            'AQAAAAIAAYagAAAAEOjjbtTv80Zr9jh2mize6u6/2NoIhvSYeOuyFQzNDuEItjxjmXNc6Ipjs1Cl51RnNg==',
                            'GAOCALE2EMTREN3T3R7OKJCLAZIAAPTV',
                            '33b8aa87-7dc2-4939-9eac-63a5e7a32ac5',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            'UcWi+LzLlm0SWx9A5iD+beFSiMc1twoHUWVPEwrNHnSS3RgusLlEltb0cX9KQOcuiRPQ8VxMPqLAd2N//j8AcA==',
                            '2025-03-10 00:06:58.3260125'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            '11843320-34ec-44f8-b848-d6b76bbcb7d0',
                            'Super Admin',
                            'Admin',
                            'admin_super99@gmail.com',
                            'ADMIN_SUPER99@GMAIL.COM',
                            'admin_super99@gmail.com',
                            'ADMIN_SUPER99@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEMMU1lRZo4x+TTYtQt4kcqiaAmi33RLcqNyyi6uxxEa9qVjlAbZGsR13S6LFplVBtA==',
                            'KUKCF26NNPKOUW33ZDDXVKBCA7OGQB74',
                            'a0b39726-83bd-4007-8fb6-0cbc0f7d0cfc',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            'SaMu3E0e7bJoqKSfYZBWs0Hm1WqPJ/2G+g9eLBXHm5sVM5vv1yxc7WYsWAnfLQq06itYj865IcwD2til1jeHCg==',
                            '2025-03-10 00:08:27.9940411'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            'faceb3a8-a10d-416b-aa4d-b8fa1d88fdb2',
                            'JOHNY',
                            'Admin',
                            'RANDOM22@GMAIL.COM',
                            'RANDOM22@GMAIL.COM',
                            'RANDOM22@GMAIL.COM',
                            'RANDOM22@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEK/GDmCeqw3dWXY2Oll5505GBnScLXwVXP0sXDwK62duS3pRQb0pbEN/xfLeAlSKWQ==',
                            '27MTBSAREPSFTUSI4CMWKODKXTV7W2AZ',
                            'd54181c1-81c2-4da7-b3ba-3eaae4247375',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            'GnvaFjYTRNqluOLV9cUkB+5FbPgTSrf9aGYDK4Yo2j6nrPIdGA4gR8mXmFwQVilL9aUy7TPpgGAsDW7iBWV9+w==',
                            '2025-03-10 00:27:57.5589105'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            'c7ab6bd4-573b-4a6b-a083-2a6ceb701353',
                            'JOHNY',
                            'Admin',
                            'jonhnnypaul22@gmail.COM',
                            'JONHNNYPAUL22@GMAIL.COM',
                            'jonhnnypaul22@gmail.COM',
                            'JONHNNYPAUL22@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEMrbYvKiLivbGBe8LnEIhAxT58PQlIfpGsV6BSLSgoMDcfb64gWeEsUKlczxO8a6yQ==',
                            '6EAVIGBG2XBIWCXBDZAD5MHPUZYH4QNE',
                            '48496ecc-9fea-4873-a533-90c3566177b0',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            'OL7I71VrjADuBWPPHwO2vz+5OQx/KfBSmiMSFBqTntm6NKxF6/3p7d+gXEujeEj8psXO7ZEvJ+XGgOeoFXaw1A==',
                            '2025-03-10 00:28:38.5051645'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            'b0a1d7e4-ff21-47ed-8205-2fd6f2e3a742',
                            'jpcv',
                            'Admin',
                            'jpcarvajal@gmail.com',
                            'JPCARVAJAL@GMAIL.COM',
                            'jpcarvajal@gmail.com',
                            'JPCARVAJAL@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEFkDh3JKJ//EPt5irbwwQOGt26CwfG5teuvnLqZkG4teXSs/P2w6GA/5EmPdS3GolA==',
                            'JBCPHHXX2LURIP52DW5YZ6IO2273XM6E',
                            '1a02d67b-cca4-4137-bf9c-569f3cc52e1d',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            'fGNxMK5Ao8XLqgeYbud38ooTxtl03CpSHOnJUBaQ3PcCODVx4tIeXqoIzRDWvR3sGkbn/mUWv84HSPzz3jShzA==',
                            '2025-03-10 00:43:33.6629918'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            '6ae6973a-c59d-478e-8cdc-5d879512f167',
                            'jpcv',
                            'Admin',
                            'jeanypaul0308@gmail.com',
                            'JEANYPAUL0308@GMAIL.COM',
                            'jeanypaul0308@gmail.com',
                            'JEANYPAUL0308@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEDcuVQcoJyE8UGjc2Nh4Dc8EG1xcYXIGXsIHq2xtKHR05fLfzBCYmJQ2lLNb/JmKAA==',
                            'DSE3CYUEQHQA2FAYTOVSUGQSGPKXHUMV',
                            '7758f9c6-0352-4f47-97dd-c01188d2d693',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            '7bpAFulOcmYMC2OMU5VKmFiRho2Lj2obhbtc62Tdbp2knrZCxusGoTXfqT/9XsMe1ZD+qaWOT/jsBrxcP1NUTQ==',
                            '2025-03-10 00:44:03.9599339'
                        );

INSERT INTO AspNetUsers (
                            Id,
                            FullName,
                            Role,
                            UserName,
                            NormalizedUserName,
                            Email,
                            NormalizedEmail,
                            EmailConfirmed,
                            PasswordHash,
                            SecurityStamp,
                            ConcurrencyStamp,
                            PhoneNumber,
                            PhoneNumberConfirmed,
                            TwoFactorEnabled,
                            LockoutEnd,
                            LockoutEnabled,
                            AccessFailedCount,
                            RefreshToken,
                            RefreshTokenExpiry
                        )
                        VALUES (
                            '592d81aa-153c-46dc-8208-a45d7a89c74e',
                            'sam',
                            'Admin',
                            'sam@gmail.com',
                            'SAM@GMAIL.COM',
                            'sam@gmail.com',
                            'SAM@GMAIL.COM',
                            0,
                            'AQAAAAIAAYagAAAAEL+qpbfqApuvo0dERvBgA8ET2JD30TUjrGlah68WvQbZj5NIOsxkXcZgUcaWFoPY0A==',
                            'CRUMNVDIEG5AJU6EBIRVNPM63W46UKK4',
                            '3bd75d8b-1902-4827-a065-f9d41303faa7',
                            NULL,
                            0,
                            0,
                            NULL,
                            1,
                            0,
                            '+YKBQqEVAdIvCDCjgUctoZxAqw44n4ndrSFcItLqVx9JTdN7g8xN6jFHkZya4Svsts/1YEE7xs5zo4OgjGRmbQ==',
                            '2025-03-10 01:01:59.7469516'
                        );


-- Table: AspNetUserTokens
DROP TABLE IF EXISTS AspNetUserTokens;

CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId        TEXT NOT NULL,
    LoginProvider TEXT NOT NULL,
    Name          TEXT NOT NULL,
    Value         TEXT NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (
        UserId,
        LoginProvider,
        Name
    ),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (
        UserId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);


-- Table: Companies
DROP TABLE IF EXISTS Companies;

CREATE TABLE IF NOT EXISTS Companies (
    Id       INTEGER NOT NULL
                     CONSTRAINT PK_Companies PRIMARY KEY AUTOINCREMENT,
    Name     TEXT    NOT NULL,
    Industry TEXT    NOT NULL,
    Location TEXT    NOT NULL
);


-- Table: CompanyRecruiters
DROP TABLE IF EXISTS CompanyRecruiters;

CREATE TABLE IF NOT EXISTS CompanyRecruiters (
    CompaniesId  INTEGER NOT NULL,
    RecruitersId TEXT    NOT NULL,
    CONSTRAINT PK_CompanyRecruiters PRIMARY KEY (
        CompaniesId,
        RecruitersId
    ),
    CONSTRAINT FK_CompanyRecruiters_AspNetUsers_RecruitersId FOREIGN KEY (
        RecruitersId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanyRecruiters_Companies_CompaniesId FOREIGN KEY (
        CompaniesId
    )
    REFERENCES Companies (Id) ON DELETE CASCADE
);


-- Table: JobApplications
DROP TABLE IF EXISTS JobApplications;

CREATE TABLE IF NOT EXISTS JobApplications (
    Id          INTEGER NOT NULL
                        CONSTRAINT PK_JobApplications PRIMARY KEY AUTOINCREMENT,
    JobId       INTEGER NOT NULL,
    JobSeekerId TEXT    NOT NULL,
    Status      TEXT    NOT NULL,
    AppliedOn   TEXT    NOT NULL,
    CONSTRAINT FK_JobApplications_AspNetUsers_JobSeekerId FOREIGN KEY (
        JobSeekerId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_JobApplications_Jobs_JobId FOREIGN KEY (
        JobId
    )
    REFERENCES Jobs (Id) ON DELETE CASCADE
);


-- Table: Jobs
DROP TABLE IF EXISTS Jobs;

CREATE TABLE IF NOT EXISTS Jobs (
    Id          INTEGER NOT NULL
                        CONSTRAINT PK_Jobs PRIMARY KEY AUTOINCREMENT,
    Title       TEXT    NOT NULL,
    Location    TEXT    NOT NULL,
    JobType     TEXT    NOT NULL,
    Salary      TEXT    NOT NULL,
    PostedOn    TEXT    NOT NULL,
    RecruiterId TEXT    NOT NULL,
    CompanyId   INTEGER NOT NULL,
    CONSTRAINT FK_Jobs_AspNetUsers_RecruiterId FOREIGN KEY (
        RecruiterId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Jobs_Companies_CompanyId FOREIGN KEY (
        CompanyId
    )
    REFERENCES Companies (Id) ON DELETE CASCADE
);


-- Table: Reviews
DROP TABLE IF EXISTS Reviews;

CREATE TABLE IF NOT EXISTS Reviews (
    Id        INTEGER NOT NULL
                      CONSTRAINT PK_Reviews PRIMARY KEY AUTOINCREMENT,
    UserId    TEXT    NOT NULL,
    CompanyId INTEGER NOT NULL,
    Rating    INTEGER NOT NULL,
    Comment   TEXT    NOT NULL,
    CreatedOn TEXT    NOT NULL,
    CONSTRAINT FK_Reviews_AspNetUsers_UserId FOREIGN KEY (
        UserId
    )
    REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Companies_CompanyId FOREIGN KEY (
        CompanyId
    )
    REFERENCES Companies (Id) ON DELETE CASCADE
);


-- Index: EmailIndex
DROP INDEX IF EXISTS EmailIndex;

CREATE INDEX IF NOT EXISTS EmailIndex ON AspNetUsers (
    "NormalizedEmail"
);


-- Index: IX_AspNetRoleClaims_RoleId
DROP INDEX IF EXISTS IX_AspNetRoleClaims_RoleId;

CREATE INDEX IF NOT EXISTS IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims (
    "RoleId"
);


-- Index: IX_AspNetUserClaims_UserId
DROP INDEX IF EXISTS IX_AspNetUserClaims_UserId;

CREATE INDEX IF NOT EXISTS IX_AspNetUserClaims_UserId ON AspNetUserClaims (
    "UserId"
);


-- Index: IX_AspNetUserLogins_UserId
DROP INDEX IF EXISTS IX_AspNetUserLogins_UserId;

CREATE INDEX IF NOT EXISTS IX_AspNetUserLogins_UserId ON AspNetUserLogins (
    "UserId"
);


-- Index: IX_AspNetUserRoles_RoleId
DROP INDEX IF EXISTS IX_AspNetUserRoles_RoleId;

CREATE INDEX IF NOT EXISTS IX_AspNetUserRoles_RoleId ON AspNetUserRoles (
    "RoleId"
);


-- Index: IX_CompanyRecruiters_RecruitersId
DROP INDEX IF EXISTS IX_CompanyRecruiters_RecruitersId;

CREATE INDEX IF NOT EXISTS IX_CompanyRecruiters_RecruitersId ON CompanyRecruiters (
    "RecruitersId"
);


-- Index: IX_JobApplications_JobId
DROP INDEX IF EXISTS IX_JobApplications_JobId;

CREATE INDEX IF NOT EXISTS IX_JobApplications_JobId ON JobApplications (
    "JobId"
);


-- Index: IX_JobApplications_JobSeekerId
DROP INDEX IF EXISTS IX_JobApplications_JobSeekerId;

CREATE INDEX IF NOT EXISTS IX_JobApplications_JobSeekerId ON JobApplications (
    "JobSeekerId"
);


-- Index: IX_Jobs_CompanyId
DROP INDEX IF EXISTS IX_Jobs_CompanyId;

CREATE INDEX IF NOT EXISTS IX_Jobs_CompanyId ON Jobs (
    "CompanyId"
);


-- Index: IX_Jobs_RecruiterId
DROP INDEX IF EXISTS IX_Jobs_RecruiterId;

CREATE INDEX IF NOT EXISTS IX_Jobs_RecruiterId ON Jobs (
    "RecruiterId"
);


-- Index: IX_Reviews_CompanyId
DROP INDEX IF EXISTS IX_Reviews_CompanyId;

CREATE INDEX IF NOT EXISTS IX_Reviews_CompanyId ON Reviews (
    "CompanyId"
);


-- Index: IX_Reviews_UserId
DROP INDEX IF EXISTS IX_Reviews_UserId;

CREATE INDEX IF NOT EXISTS IX_Reviews_UserId ON Reviews (
    "UserId"
);


-- Index: RoleNameIndex
DROP INDEX IF EXISTS RoleNameIndex;

CREATE UNIQUE INDEX IF NOT EXISTS RoleNameIndex ON AspNetRoles (
    "NormalizedName"
);


-- Index: UserNameIndex
DROP INDEX IF EXISTS UserNameIndex;

CREATE UNIQUE INDEX IF NOT EXISTS UserNameIndex ON AspNetUsers (
    "NormalizedUserName"
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
