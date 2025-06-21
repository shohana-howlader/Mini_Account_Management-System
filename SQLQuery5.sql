-- Drop procedures if they exist to allow safe re-creation
IF OBJECT_ID('sp_InsertScreen', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertScreen;
GO

CREATE PROCEDURE sp_InsertScreen
    @ScreenName NVARCHAR(100),
    @URL NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Screens (ScreenName, URL)
    VALUES (@ScreenName, @URL);

    -- Return the newly created Screen ID
    SELECT SCOPE_IDENTITY() AS NewScreenId;
END;
GO


IF OBJECT_ID('sp_UpdateScreen', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateScreen;
GO

CREATE PROCEDURE sp_UpdateScreen
    @Id INT,
    @ScreenName NVARCHAR(100),
    @URL NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Screens
    SET ScreenName = @ScreenName,
        URL = @URL
    WHERE Id = @Id;

    -- Return the number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO


IF OBJECT_ID('sp_DeleteScreen', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteScreen;
GO

CREATE PROCEDURE sp_DeleteScreen
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Delete related permissions first
        DELETE FROM UserRolePermissions WHERE ScreenId = @Id;

        -- Delete the screen
        DELETE FROM Screens WHERE Id = @Id;

        COMMIT TRANSACTION;

        -- Return number of rows affected on Screens table
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Rethrow error to caller
        THROW;
    END CATCH
END;
GO


IF OBJECT_ID('sp_GetAllScreens', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllScreens;
GO

CREATE PROCEDURE sp_GetAllScreens
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ScreenName, URL
    FROM Screens
    ORDER BY ScreenName;
END;
GO
