CREATE PROCEDURE sp_GetUserById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserName,
        Password,
        CreatedDate
    FROM Users
    WHERE Id = @Id;
END
GO

-- =============================================
-- 2. GET ALL USERS
-- =============================================
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserName,
        Password,
        CreatedDate
    FROM Users
    ORDER BY CreatedDate DESC;
END
GO

-- =============================================
-- 3. INSERT NEW USER
-- =============================================
CREATE PROCEDURE sp_InsertUser
    @UserName NVARCHAR(100),
    @Password NVARCHAR(255),
    @CreatedDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Set default created date if not provided
    IF @CreatedDate IS NULL
        SET @CreatedDate = GETDATE();
    
    DECLARE @NewUserId INT;
    
    INSERT INTO Users (UserName, Password, CreatedDate)
    VALUES (@UserName, @Password, @CreatedDate);
    
    SET @NewUserId = SCOPE_IDENTITY();
    
    -- Return the newly created user
    SELECT 
        Id,
        UserName,
        Password,
        CreatedDate
    FROM Users
    WHERE Id = @NewUserId;
END
GO

-- =============================================
-- 4. UPDATE USER
-- =============================================
CREATE PROCEDURE sp_UpdateUser
    @Id INT,
    @UserName NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsAffected INT;
    
    UPDATE Users 
    SET 
        UserName = @UserName,
        Password = @Password
    WHERE Id = @Id;
    
    SET @RowsAffected = @@ROWCOUNT;
    
    -- Return updated user if update was successful
    IF @RowsAffected > 0
    BEGIN
        SELECT 
            Id,
            UserName,
            Password,
            CreatedDate
        FROM Users
        WHERE Id = @Id;
    END
    ELSE
    BEGIN
        -- Return empty result set if user not found
        SELECT 
            Id,
            UserName,
            Password,
            CreatedDate
        FROM Users
        WHERE 1 = 0;
    END
END
GO

-- =============================================
-- 5. DELETE USER
-- =============================================
CREATE PROCEDURE sp_DeleteUser
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsAffected INT;
    
    DELETE FROM Users 
    WHERE Id = @Id;
    
    SET @RowsAffected = @@ROWCOUNT;
    
    -- Return number of rows affected
    SELECT @RowsAffected as RowsAffected;
END
GO
