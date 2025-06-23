USE [AnfDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Updated procedure to include all users, even without roles
ALTER PROCEDURE [dbo].[sp_GetAllUserRoleMappings]
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

ALTER PROCEDURE sp_InsertUserRoleMapping
    @UserId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM UserRoleMappings WHERE UserId = @UserId)
    BEGIN
        -- User already has a role
        RAISERROR('This user already has a role assigned.', 16, 1);
        RETURN;
    END

    INSERT INTO UserRoleMappings (UserId, RoleId)
    VALUES (@UserId, @RoleId)
END
