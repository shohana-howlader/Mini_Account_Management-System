USE [AnfDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUser]    Script Date: 6/29/2025 4:25:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. UPDATE USER
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateUser]
    @Id INT,
    @FullName NVARCHAR(100),
	 @UserName NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsAffected INT;
    
    UPDATE Users 
    SET 
		FullName = @FullName,
        UserName = @UserName,
        Password = @Password
    WHERE Id = @Id;
    
    SET @RowsAffected = @@ROWCOUNT;
    
    -- Return updated user if update was successful
    IF @RowsAffected > 0
    BEGIN
        SELECT 
            Id,
			FullName,
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
			FullName,
            UserName,
            Password,
            CreatedDate
        FROM Users
        WHERE 1 = 0;
    END
END
