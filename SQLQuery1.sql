CREATE  PROCEDURE sp_GetRoles
AS
BEGIN
    SELECT Id, RoleName, Description, CreatedDate
    FROM Roles
    ORDER BY Id;
END
GO


CREATE  PROCEDURE sp_InsertRole
    @RoleName NVARCHAR(100),
    @Description NVARCHAR(255),
    @CreatedDate DATETIME
AS
BEGIN
    INSERT INTO Roles (RoleName, Description, CreatedDate)
    VALUES (@RoleName, @Description, @CreatedDate);
END
GO


CREATE  PROCEDURE sp_UpdateRole
    @Id INT,
    @RoleName NVARCHAR(100),
    @Description NVARCHAR(255),
    @CreatedDate DATETIME
AS
BEGIN
    UPDATE Roles
    SET RoleName = @RoleName,
        Description = @Description,
        CreatedDate = @CreatedDate
    WHERE Id = @Id;
END
GO


CREATE  PROCEDURE sp_DeleteRole
    @Id INT
AS
BEGIN
    DELETE FROM Roles
    WHERE Id = @Id;
END
GO


CREATE  PROCEDURE sp_GetRoleById
    @Id INT
AS
BEGIN
    SELECT Id, RoleName, Description, CreatedDate
    FROM Roles
    WHERE Id = @Id;
END
GO


