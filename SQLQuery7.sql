CREATE PROCEDURE sp_GetAllUserRoleMappings
AS
BEGIN
    SELECT 
        urm.Id,
        u.Id AS UserId,
        u.UserName,
        r.RoleName
    FROM Users u
    LEFT JOIN UserRoleMappings urm ON urm.UserId = u.Id
    LEFT JOIN Roles r ON urm.RoleId = r.Id
END
GO

CREATE PROCEDURE sp_InsertUserRoleMapping
    @UserId INT,
    @RoleId INT
AS
BEGIN
    INSERT INTO UserRoleMappings (UserId, RoleId)
    VALUES (@UserId, @RoleId)
END


CREATE PROCEDURE sp_UpdateUserRoleMapping
    @Id INT,
    @UserId INT,
    @RoleId INT
AS
BEGIN
    UPDATE UserRoleMappings
    SET UserId = @UserId,
        RoleId = @RoleId
    WHERE Id = @Id
END


CREATE PROCEDURE sp_DeleteUserRoleMapping
    @Id INT
AS
BEGIN
    DELETE FROM UserRoleMappings
    WHERE Id = @Id
END

CREATE PROCEDURE sp_GetUserRoleMappingById
    @Id INT
AS
BEGIN
    SELECT urm.Id, u.UserName, r.RoleName, urm.UserId, urm.RoleId
    FROM UserRoleMappings urm
    INNER JOIN Users u ON urm.UserId = u.Id
    INNER JOIN Roles r ON urm.RoleId = r.Id
    WHERE urm.Id = @Id
END

