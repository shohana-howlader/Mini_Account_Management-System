USE [AnfDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserById]    Script Date: 6/29/2025 4:26:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_GetUserById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
		FullName,
        UserName,
        Password,
        CreatedDate
    FROM Users
    WHERE Id = @Id;
END
