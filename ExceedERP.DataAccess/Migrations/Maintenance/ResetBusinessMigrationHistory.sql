-- One-time, history-only reconciliation approved for this client database.
-- Does NOT apply migrations, change application tables, or fix schema differences.
-- Retains the existing Model and ProductVersion of the three recorded local migrations.
-- The fourth local migration (LiveEntitlementChangesMigration) remains PENDING.
-- User.__MigrationHistory and records belonging to other contexts are untouched.
-- Stop the application and any migration jobs before running this maintenance script.
-- Keep the backup table until the reconciliation has been accepted.

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'ExceedERP2021-MPE-PRO'
    THROW 50001, 'Wrong database. Expected ExceedERP2021-MPE-PRO.', 1;

DECLARE @ContextKey nvarchar(300) = N'ExceedERP.DataAccess.Migrations.Configuration';
DECLARE @Keep TABLE (MigrationId nvarchar(150) NOT NULL PRIMARY KEY);
INSERT INTO @Keep (MigrationId) VALUES
    (N'202607270956152_Initial'),
    (N'202609142150212_HRandothermodelchanges'),
    (N'202609220901537_fleetandmaaintenacechanges');

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @BeforeCount int;
    -- Prevent concurrent history changes between validation, backup and deletion.
    SELECT @BeforeCount = COUNT(*)
    FROM [EXCEED].[__MigrationHistory] WITH (TABLOCKX, HOLDLOCK)
    WHERE ContextKey = @ContextKey;

    IF @BeforeCount <> 188
        THROW 50002, 'History changed since inspection. Reinspect before proceeding.', 1;

    IF EXISTS (
        SELECT 1 FROM @Keep k
        WHERE NOT EXISTS (
            SELECT 1 FROM [EXCEED].[__MigrationHistory] h
            WHERE h.ContextKey = @ContextKey AND h.MigrationId = k.MigrationId
        )
    )
        THROW 50003, 'A retained local migration is not recorded. Nothing was deleted.', 1;

    IF EXISTS (
        SELECT 1 FROM [EXCEED].[__MigrationHistory]
        WHERE ContextKey = @ContextKey
          AND MigrationId > N'202609220901537_fleetandmaaintenacechanges'
    )
        THROW 50004, 'A newer migration is recorded. Do not remove its history.', 1;

    IF OBJECT_ID(N'EXCEED.__MigrationHistoryBackup_20261002_BusinessReset', N'U') IS NOT NULL
        THROW 50005, 'Backup table already exists. Do not overwrite it.', 1;

    SELECT MigrationId, ContextKey, Model, ProductVersion
    INTO [EXCEED].[__MigrationHistoryBackup_20261002_BusinessReset]
    FROM [EXCEED].[__MigrationHistory]
    WHERE ContextKey = @ContextKey;

    IF (SELECT COUNT(*) FROM [EXCEED].[__MigrationHistoryBackup_20261002_BusinessReset]) <> @BeforeCount
        THROW 50006, 'History backup validation failed.', 1;

    DELETE h
    FROM [EXCEED].[__MigrationHistory] h
    WHERE h.ContextKey = @ContextKey
      AND NOT EXISTS (SELECT 1 FROM @Keep k WHERE k.MigrationId = h.MigrationId);

    DECLARE @DeletedCount int = @@ROWCOUNT;

    IF @DeletedCount <> 185 OR (
        SELECT COUNT(*) FROM [EXCEED].[__MigrationHistory] WHERE ContextKey = @ContextKey
    ) <> 3
        THROW 50007, 'Unexpected history result. Changes will be rolled back.', 1;

    -- Verify all retained metadata is byte-for-byte unchanged.
    IF EXISTS (
        SELECT 1
        FROM [EXCEED].[__MigrationHistory] h
        JOIN [EXCEED].[__MigrationHistoryBackup_20261002_BusinessReset] b
          ON b.ContextKey = h.ContextKey AND b.MigrationId = h.MigrationId
        WHERE h.ContextKey = @ContextKey
          AND (h.Model <> b.Model OR h.ProductVersion <> b.ProductVersion)
    )
        THROW 50008, 'Retained migration metadata changed. Changes will be rolled back.', 1;

    COMMIT TRANSACTION;

    SELECT @BeforeCount AS BackedUpRecords, @DeletedCount AS RemovedRecords,
           N'EXCEED.__MigrationHistoryBackup_20261002_BusinessReset' AS BackupTable;
    SELECT MigrationId, ContextKey, ProductVersion
    FROM [EXCEED].[__MigrationHistory]
    WHERE ContextKey = @ContextKey
    ORDER BY MigrationId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

-- Emergency restore (review and run separately while migration jobs are stopped):
-- BEGIN TRANSACTION;
-- DELETE FROM [EXCEED].[__MigrationHistory]
-- WHERE ContextKey = N'ExceedERP.DataAccess.Migrations.Configuration';
-- INSERT INTO [EXCEED].[__MigrationHistory] (MigrationId, ContextKey, Model, ProductVersion)
-- SELECT MigrationId, ContextKey, Model, ProductVersion
-- FROM [EXCEED].[__MigrationHistoryBackup_20261002_BusinessReset];
-- COMMIT TRANSACTION;