CREATE OR ALTER PROCEDURE sp_GetRoles
AS
BEGIN
    SELECT Id, RoleName, CreatedDate
    FROM Roles
    ORDER BY Id;
END
GO

-- DROP PROCEDURE IF EXISTS sp_GetRoles
CREATE OR ALTER PROCEDURE sp_InsertRole
    @RoleName NVARCHAR(100),
    @CreatedDate DATETIME
AS
BEGIN
    INSERT INTO Roles (RoleName, CreatedDate)
    VALUES (@RoleName, @CreatedDate);
END
GO

-- DROP PROCEDURE IF EXISTS sp_UpdateRole
CREATE OR ALTER PROCEDURE sp_UpdateRole
    @Id INT,
    @RoleName NVARCHAR(100),
    @CreatedDate DATETIME
AS
BEGIN
    UPDATE Roles
    SET RoleName = @RoleName,
        CreatedDate = @CreatedDate
    WHERE Id = @Id;
END
GO

-- DROP PROCEDURE IF EXISTS sp_DeleteRole
CREATE OR ALTER PROCEDURE sp_DeleteRole
    @Id INT
AS
BEGIN
    DELETE FROM Roles
    WHERE Id = @Id;
END
GO