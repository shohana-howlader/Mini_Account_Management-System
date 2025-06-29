USE [AnfDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertMultipleUserRolePermissions]    Script Date: 6/25/2025 10:37:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_InsertMultipleUserRolePermissions]
    @UserId INT,
    @RoleId INT,
    @PermissionJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Delete old permissions
    IF @UserId IS NOT NULL AND @UserId != 0
    BEGIN
        DELETE FROM UserRolePermissions WHERE UserId = @UserId;
    END

    IF @RoleId IS NOT NULL AND @RoleId != 0
    BEGIN
        DELETE FROM UserRolePermissions WHERE RoleId = @RoleId;
    END

    INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
    SELECT 
        @UserId,
        @RoleId,
        j.ScreenId,
        j.CanRead,
        j.CanWrite,
        j.CanEdit,
        j.CanDelete
    FROM OPENJSON(@PermissionJson)
    WITH (
        ScreenId INT,
        CanRead BIT,
        CanWrite BIT,
        CanEdit BIT,
        CanDelete BIT
    ) AS j
    WHERE 
        j.ScreenId IS NOT NULL AND
        (
            j.CanRead IS NOT NULL OR
            j.CanWrite IS NOT NULL OR
            j.CanEdit IS NOT NULL OR
            j.CanDelete IS NOT NULL
        )
END;
