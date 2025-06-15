using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Account_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class store : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region User Stored Procedures

            // Insert User
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_InsertUser
                    @UserName NVARCHAR(100),
                    @Password NVARCHAR(255),
                    @RoleId INT,
                    @CreatedDate DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO Users (UserName, Password, RoleId, CreatedDate)
                    VALUES (@UserName, @Password, @RoleId, @CreatedDate);
                    
                    SELECT SCOPE_IDENTITY() AS NewUserId;
                END
            ");

            // Update User
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdateUser
                    @Id INT,
                    @UserName NVARCHAR(100),
                    @Password NVARCHAR(255),
                    @RoleId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    UPDATE Users 
                    SET UserName = @UserName,
                        Password = @Password,
                        RoleId = @RoleId
                    WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Delete User
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DeleteUser
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- First delete related permissions
                    DELETE FROM UserRolePermissions WHERE UserId = @Id;
                    
                    -- Then delete user
                    DELETE FROM Users WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Get User by ID
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetUserById
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT u.Id, u.UserName, u.Password, u.RoleId, u.CreatedDate,
                           r.RoleName
                    FROM Users u
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Id = @Id;
                END
            ");

            // Get All Users
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetAllUsers
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT u.Id, u.UserName, u.Password, u.RoleId, u.CreatedDate,
                           r.RoleName
                    FROM Users u
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    ORDER BY u.CreatedDate DESC;
                END
            ");

            // User Login
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UserLogin
                    @UserName NVARCHAR(100),
                    @Password NVARCHAR(255)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT u.Id, u.UserName, u.RoleId, u.CreatedDate,
                           r.RoleName
                    FROM Users u
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.UserName = @UserName AND u.Password = @Password;
                END
            ");

            #endregion

            #region Role Stored Procedures

            // Insert Role
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_InsertRole
                    @RoleName NVARCHAR(100),
                    @CreatedDate DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO Roles (RoleName, CreatedDate)
                    VALUES (@RoleName, @CreatedDate);
                    
                    SELECT SCOPE_IDENTITY() AS NewRoleId;
                END
            ");

            // Update Role
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdateRole
                    @Id INT,
                    @RoleName NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    UPDATE Roles 
                    SET RoleName = @RoleName
                    WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Delete Role
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DeleteRole
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- Check if role is being used
                    IF EXISTS (SELECT 1 FROM Users WHERE RoleId = @Id)
                    BEGIN
                        RAISERROR('Cannot delete role. It is being used by users.', 16, 1);
                        RETURN;
                    END
                    
                    -- Delete related permissions first
                    DELETE FROM UserRolePermissions WHERE RoleId = @Id;
                    
                    -- Delete role
                    DELETE FROM Roles WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Get All Roles
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetAllRoles
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT Id, RoleName, CreatedDate
                    FROM Roles
                    ORDER BY RoleName;
                END
            ");

            #endregion

            #region Screen Stored Procedures

            // Insert Screen
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_InsertScreen
                    @ScreenName NVARCHAR(100),
                    @URL NVARCHAR(255)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO Screens (ScreenName, URL)
                    VALUES (@ScreenName, @URL);
                    
                    SELECT SCOPE_IDENTITY() AS NewScreenId;
                END
            ");

            // Update Screen
            migrationBuilder.Sql(@"
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
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Delete Screen
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DeleteScreen
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- Delete related permissions first
                    DELETE FROM UserRolePermissions WHERE ScreenId = @Id;
                    
                    -- Delete screen
                    DELETE FROM Screens WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Get All Screens
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetAllScreens
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT Id, ScreenName, URL
                    FROM Screens
                    ORDER BY ScreenName;
                END
            ");

            #endregion

            #region UserRolePermissions Stored Procedures

            // Insert User Role Permission
            migrationBuilder.Sql(@"
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
                    
                    -- Check if permission already exists
                    IF EXISTS (SELECT 1 FROM UserRolePermissions 
                              WHERE UserId = @UserId AND RoleId = @RoleId AND ScreenId = @ScreenId)
                    BEGIN
                        -- Update existing permission
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
                        -- Insert new permission
                        INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
                        VALUES (@UserId, @RoleId, @ScreenId, @CanRead, @CanWrite, @CanEdit, @CanDelete);
                        
                        SELECT SCOPE_IDENTITY() AS NewPermissionId;
                    END
                END
            ");

            // Update User Role Permission
            migrationBuilder.Sql(@"
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
            ");

            // Delete User Role Permission
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DeleteUserRolePermission
                    @Id INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    DELETE FROM UserRolePermissions WHERE Id = @Id;
                    
                    SELECT @@ROWCOUNT AS RowsAffected;
                END
            ");

            // Get User Permissions
            migrationBuilder.Sql(@"
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
            ");

            // Get Role Permissions
            migrationBuilder.Sql(@"
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
            ");

            // Get All Permissions
            migrationBuilder.Sql(@"
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
            ");

            // Check User Screen Permission
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_CheckUserScreenPermission
                    @UserId INT,
                    @ScreenId INT,
                    @PermissionType NVARCHAR(10) -- 'Read', 'Write', 'Edit', 'Delete'
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    DECLARE @HasPermission BIT = 0;
                    
                    IF @PermissionType = 'Read'
                        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
                        FROM UserRolePermissions 
                        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanRead = 1;
                    ELSE IF @PermissionType = 'Write'
                        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
                        FROM UserRolePermissions 
                        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanWrite = 1;
                    ELSE IF @PermissionType = 'Edit'
                        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
                        FROM UserRolePermissions 
                        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanEdit = 1;
                    ELSE IF @PermissionType = 'Delete'
                        SELECT @HasPermission = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
                        FROM UserRolePermissions 
                        WHERE UserId = @UserId AND ScreenId = @ScreenId AND CanDelete = 1;
                    
                    SELECT @HasPermission AS HasPermission;
                END
            ");

            #endregion

            #region Utility Stored Procedures

            // Get Dashboard Data
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetDashboardData
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    SELECT 
                        (SELECT COUNT(*) FROM Users) AS TotalUsers,
                        (SELECT COUNT(*) FROM Roles) AS TotalRoles,
                        (SELECT COUNT(*) FROM Screens) AS TotalScreens,
                        (SELECT COUNT(*) FROM UserRolePermissions) AS TotalPermissions;
                END
            ");

            // Get User with Permissions
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetUserWithPermissions
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- User Info
                    SELECT u.Id, u.UserName, u.RoleId, u.CreatedDate, r.RoleName
                    FROM Users u
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Id = @UserId;
                    
                    -- User Permissions
                    SELECT p.Id, p.ScreenId, p.CanRead, p.CanWrite, p.CanEdit, p.CanDelete,
                           s.ScreenName, s.URL
                    FROM UserRolePermissions p
                    INNER JOIN Screens s ON p.ScreenId = s.Id
                    WHERE p.UserId = @UserId
                    ORDER BY s.ScreenName;
                END
            ");

            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop all stored procedures
            var procedures = new[]
            {
                "sp_InsertUser", "sp_UpdateUser", "sp_DeleteUser", "sp_GetUserById", "sp_GetAllUsers", "sp_UserLogin",
                "sp_InsertRole", "sp_UpdateRole", "sp_DeleteRole", "sp_GetAllRoles",
                "sp_InsertScreen", "sp_UpdateScreen", "sp_DeleteScreen", "sp_GetAllScreens",
                "sp_InsertUserRolePermission", "sp_UpdateUserRolePermission", "sp_DeleteUserRolePermission",
                "sp_GetUserPermissions", "sp_GetRolePermissions", "sp_GetAllUserRolePermissions",
                "sp_CheckUserScreenPermission", "sp_GetDashboardData", "sp_GetUserWithPermissions"
            };

            foreach (var procedure in procedures)
            {
                migrationBuilder.Sql($"DROP PROCEDURE IF EXISTS {procedure}");
            }
        }
    }
}
