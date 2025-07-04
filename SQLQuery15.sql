USE [AnfDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUser]    Script Date: 6/29/2025 4:22:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 3. INSERT NEW USER
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertUser]
	@FullName NVARCHAR(100),
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
    
    INSERT INTO Users ( FullName, UserName, Password, CreatedDate)
    VALUES (@FullName, @UserName, @Password, @CreatedDate);
    
    SET @NewUserId = SCOPE_IDENTITY();
    
    -- Return the newly created user
    SELECT 
        Id,
		FullName,
        UserName,
        Password,
        CreatedDate
    FROM Users
    WHERE Id = @NewUserId;
END
