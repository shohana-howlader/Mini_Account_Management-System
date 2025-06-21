-- =============================================
-- sp_InsertUserRolePermission
-- =============================================
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
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM UserRolePermissions
        WHERE UserId = @UserId AND RoleId = @RoleId AND ScreenId = @ScreenId
    )
    BEGIN
        UPDATE UserRolePermissions
        SET CanRead = @CanRead,
            CanWrite = @CanWrite,
            CanEdit = @CanEdit,
            CanDelete = @CanDelete
        WHERE UserId = @UserId AND RoleId = @RoleId AND ScreenId = @ScreenId;

        SELECT 'Updated' AS Result;
    END
    ELSE
    BEGIN
        INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
        VALUES (@UserId, @RoleId, @ScreenId, @CanRead, @CanWrite, @CanEdit, @CanDelete);

        SELECT SCOPE_IDENTITY() AS NewPermissionId;
    END
END
GO

-- =============================================
-- sp_UpdateUserRolePermission
-- =============================================
CREATE PROCEDURE sp_UpdateUserRolePermission
    @Id INT,
    @CanRead BIT,
    @CanWrite BIT,
    @CanEdit BIT,
    @CanDelete BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UserRolePermissions
    SET CanRead = @CanRead,
        CanWrite = @CanWrite,
        CanEdit = @CanEdit,
        CanDelete = @CanDelete
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- sp_DeleteUserRolePermission
-- =============================================
CREATE PROCEDURE sp_DeleteUserRolePermission
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM UserRolePermissions
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- sp_GetUserPermissions
-- =============================================
CREATE PROCEDURE sp_GetUserPermissions
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.UserId, p.RoleId, p.ScreenId,
           p.CanRead, p.CanWrite, p.CanEdit, p.CanDelete,
           u.UserName, r.RoleName, s.ScreenName, s.URL
    FROM UserRolePermissions p
    INNER JOIN Users u ON p.UserId = u.Id
    INNER JOIN Roles r ON p.RoleId = r.Id
    INNER JOIN Screens s ON p.ScreenId = s.Id
    WHERE p.UserId = @UserId
    ORDER BY s.ScreenName;
END
GO

-- =============================================
-- sp_GetRolePermissions
-- =============================================
CREATE PROCEDURE sp_GetRolePermissions
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.UserId, p.RoleId, p.ScreenId,
           p.CanRead, p.CanWrite, p.CanEdit, p.CanDelete,
           u.UserName, r.RoleName, s.ScreenName, s.URL
    FROM UserRolePermissions p
    INNER JOIN Users u ON p.UserId = u.Id
    INNER JOIN Roles r ON p.RoleId = r.Id
    INNER JOIN Screens s ON p.ScreenId = s.Id
    WHERE p.RoleId = @RoleId
    ORDER BY u.UserName, s.ScreenName;
END
GO

-- =============================================
-- sp_GetAllUserRolePermissions
-- =============================================
CREATE PROCEDURE sp_GetAllUserRolePermissions
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.UserId, p.RoleId, p.ScreenId,
           p.CanRead, p.CanWrite, p.CanEdit, p.CanDelete,
           u.UserName, r.RoleName, s.ScreenName, s.URL
    FROM UserRolePermissions p
    INNER JOIN Users u ON p.UserId = u.Id
    INNER JOIN Roles r ON p.RoleId = r.Id
    INNER JOIN Screens s ON p.ScreenId = s.Id
    ORDER BY u.UserName, s.ScreenName;
END
GO

-- =============================================
-- sp_CheckUserScreenPermission
-- =============================================
CREATE PROCEDURE sp_CheckUserScreenPermission
    @UserId INT,
    @ScreenId INT,
    @PermissionType NVARCHAR(10)  -- 'Read', 'Write', 'Edit', 'Delete'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasPermission BIT = 0;

    IF @PermissionType = 'Read'
    BEGIN
        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
        FROM UserRolePermissions
        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanRead = 1;
    END
    ELSE IF @PermissionType = 'Write'
    BEGIN
        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
        FROM UserRolePermissions
        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanWrite = 1;
    END
    ELSE IF @PermissionType = 'Edit'
    BEGIN
        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
        FROM UserRolePermissions
        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanEdit = 1;
    END
    ELSE IF @PermissionType = 'Delete'
    BEGIN
        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
        FROM UserRolePermissions
        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanDelete = 1;
    END

    SELECT @HasPermission AS HasPermission;
END
GO
