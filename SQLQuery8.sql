-- =============================================
-- sp_InsertUserRolePermission (Modified for Multiple Screens)
-- =============================================
DROP PROCEDURE IF EXISTS sp_InsertUserRolePermission;
GO

CREATE PROCEDURE sp_InsertUserRolePermission
    @UserId INT,
    @RoleId INT,
    @PermissionsXML XML
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ResultsTable TABLE (
        ScreenId INT,
        Action NVARCHAR(20),
        PermissionId INT
    );

    -- Parse XML and process each screen permission
    INSERT INTO @ResultsTable (ScreenId, Action, PermissionId)
    SELECT 
        T.c.value('@ScreenId', 'INT') AS ScreenId,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM UserRolePermissions
                WHERE UserId = @UserId 
                AND RoleId = @RoleId 
                AND ScreenId = T.c.value('@ScreenId', 'INT')
            ) THEN 'Updated'
            ELSE 'Inserted'
        END AS Action,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM UserRolePermissions
                WHERE UserId = @UserId 
                AND RoleId = @RoleId 
                AND ScreenId = T.c.value('@ScreenId', 'INT')
            ) THEN 
                (SELECT Id FROM UserRolePermissions
                 WHERE UserId = @UserId 
                 AND RoleId = @RoleId 
                 AND ScreenId = T.c.value('@ScreenId', 'INT'))
            ELSE 0
        END AS PermissionId
    FROM @PermissionsXML.nodes('/Permissions/Permission') T(c);

    -- Update existing permissions
    UPDATE p
    SET CanRead = T.c.value('@CanRead', 'BIT'),
        CanWrite = T.c.value('@CanWrite', 'BIT'),
        CanEdit = T.c.value('@CanEdit', 'BIT'),
        CanDelete = T.c.value('@CanDelete', 'BIT')
    FROM UserRolePermissions p
    INNER JOIN @PermissionsXML.nodes('/Permissions/Permission') T(c)
        ON p.ScreenId = T.c.value('@ScreenId', 'INT')
    WHERE p.UserId = @UserId AND p.RoleId = @RoleId;

    -- Insert new permissions
    INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
    SELECT 
        @UserId,
        @RoleId,
        T.c.value('@ScreenId', 'INT'),
        T.c.value('@CanRead', 'BIT'),
        T.c.value('@CanWrite', 'BIT'),
        T.c.value('@CanEdit', 'BIT'),
        T.c.value('@CanDelete', 'BIT')
    FROM @PermissionsXML.nodes('/Permissions/Permission') T(c)
    WHERE NOT EXISTS (
        SELECT 1 FROM UserRolePermissions
        WHERE UserId = @UserId 
        AND RoleId = @RoleId 
        AND ScreenId = T.c.value('@ScreenId', 'INT')
    );

    -- Update the results table with new IDs for inserted records
    UPDATE r
    SET PermissionId = p.Id
    FROM @ResultsTable r
    INNER JOIN UserRolePermissions p ON r.ScreenId = p.ScreenId
    WHERE r.Action = 'Inserted' 
    AND p.UserId = @UserId 
    AND p.RoleId = @RoleId
    AND r.PermissionId = 0;

    -- Return results
    SELECT ScreenId, Action, PermissionId FROM @ResultsTable;
END
GO

-- =============================================
-- Alternative: Table-Valued Parameter Approach
-- =============================================

-- First, create a user-defined table type
DROP TYPE IF EXISTS UserRolePermissionTableType;
GO

CREATE TYPE UserRolePermissionTableType AS TABLE
(
    ScreenId INT,
    CanRead BIT,
    CanWrite BIT,
    CanEdit BIT,
    CanDelete BIT
);
GO

-- Then create the stored procedure using the table type
CREATE PROCEDURE sp_InsertMultipleUserRolePermissions_TVP
    @UserId INT,
    @RoleId INT,
    @Permissions UserRolePermissionTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ResultsTable TABLE (
        ScreenId INT,
        Action NVARCHAR(20),
        PermissionId INT
    );

    -- Prepare results table
    INSERT INTO @ResultsTable (ScreenId, Action, PermissionId)
    SELECT 
        p.ScreenId,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM UserRolePermissions urp
                WHERE urp.UserId = @UserId 
                AND urp.RoleId = @RoleId 
                AND urp.ScreenId = p.ScreenId
            ) THEN 'Updated'
            ELSE 'Inserted'
        END AS Action,
        ISNULL((SELECT Id FROM UserRolePermissions urp
                WHERE urp.UserId = @UserId 
                AND urp.RoleId = @RoleId 
                AND urp.ScreenId = p.ScreenId), 0) AS PermissionId
    FROM @Permissions p;

    -- Update existing permissions
    UPDATE urp
    SET CanRead = p.CanRead,
        CanWrite = p.CanWrite,
        CanEdit = p.CanEdit,
        CanDelete = p.CanDelete
    FROM UserRolePermissions urp
    INNER JOIN @Permissions p ON urp.ScreenId = p.ScreenId
    WHERE urp.UserId = @UserId AND urp.RoleId = @RoleId;

    -- Insert new permissions
    INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
    SELECT 
        @UserId,
        @RoleId,
        p.ScreenId,
        p.CanRead,
        p.CanWrite,
        p.CanEdit,
        p.CanDelete
    FROM @Permissions p
    WHERE NOT EXISTS (
        SELECT 1 FROM UserRolePermissions urp
        WHERE urp.UserId = @UserId 
        AND urp.RoleId = @RoleId 
        AND urp.ScreenId = p.ScreenId
    );

    -- Update the results table with new IDs for inserted records
    UPDATE r
    SET PermissionId = urp.Id
    FROM @ResultsTable r
    INNER JOIN UserRolePermissions urp ON r.ScreenId = urp.ScreenId
    WHERE r.Action = 'Inserted' 
    AND urp.UserId = @UserId 
    AND urp.RoleId = @RoleId
    AND r.PermissionId = 0;

    -- Return results
    SELECT ScreenId, Action, PermissionId FROM @ResultsTable;
END
GO