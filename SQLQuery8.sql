-- ===============================
-- DROP PROCEDURES IF EXISTS
-- ===============================
IF OBJECT_ID('sp_GetUserRolePermissions', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetUserRolePermissions;
GO

IF OBJECT_ID('sp_InsertUserRolePermission', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertUserRolePermission;
GO

IF OBJECT_ID('sp_UpdateUserRolePermission', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateUserRolePermission;
GO

IF OBJECT_ID('sp_DeleteUserRolePermissions', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteUserRolePermissions;
GO

-- ===============================
-- CREATE: Get User Role Permissions
-- ===============================
CREATE PROCEDURE sp_GetUserRolePermissions
    @UserId INT = NULL,
    @RoleId INT = NULL
AS
BEGIN
    SELECT 
        urp.Id,
        urp.UserId,
        u.UserName,
        urp.RoleId,
        r.RoleName,
        urp.ScreenId,
        s.ScreenName,
        urp.CanRead,
        urp.CanWrite,
        urp.CanEdit,
        urp.CanDelete
    FROM UserRolePermissions urp
    INNER JOIN Users u ON urp.UserId = u.Id
    INNER JOIN Roles r ON urp.RoleId = r.Id
    INNER JOIN Screens s ON urp.ScreenId = s.Id
    WHERE (@UserId IS NULL OR urp.UserId = @UserId)
      AND (@RoleId IS NULL OR urp.RoleId = @RoleId)
END;
GO

-- ===============================
-- CREATE: Insert User Role Permission
-- ===============================
CREATE PROCEDURE sp_InsertUserRolePermission
    @UserId INT,
    @RoleId INT,
    @ScreenId INT,
    @CanRead BIT,
    @CanWrite BIT,
    @CanEdit BIT,
    @CanDelete BIT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM UserRolePermissions
        WHERE UserId = @UserId AND RoleId = @RoleId AND ScreenId = @ScreenId
    )
    BEGIN
        INSERT INTO UserRolePermissions 
            (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
        VALUES 
            (@UserId, @RoleId, @ScreenId, @CanRead, @CanWrite, @CanEdit, @CanDelete)
    END
END;
GO

-- ===============================
-- CREATE: Update User Role Permission
-- ===============================
CREATE PROCEDURE sp_UpdateUserRolePermission
    @UserId INT,
    @RoleId INT,
    @ScreenId INT,
    @CanRead BIT,
    @CanWrite BIT,
    @CanEdit BIT,
    @CanDelete BIT
AS
BEGIN
    UPDATE UserRolePermissions
    SET 
        CanRead = @CanRead,
        CanWrite = @CanWrite,
        CanEdit = @CanEdit,
        CanDelete = @CanDelete
    WHERE UserId = @UserId AND RoleId = @RoleId AND ScreenId = @ScreenId
END;
GO

-- ===============================
-- CREATE: Delete User Role Permissions
-- ===============================
CREATE PROCEDURE sp_DeleteUserRolePermissions
    @UserId INT = NULL,
    @RoleId INT = NULL
AS
BEGIN
    DELETE FROM UserRolePermissions
    WHERE (@UserId IS NULL OR UserId = @UserId)
      AND (@RoleId IS NULL OR RoleId = @RoleId)
END;
GO
