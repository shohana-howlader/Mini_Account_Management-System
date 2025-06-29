USE [AnfDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertMultipleUserRolePermissions]    Script Date: 6/23/2025 6:34:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_InsertMultipleUserRolePermissions]
    @UserId INT,
    @RoleId INT,
    @Permissionjson NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO UserRolePermissions (UserId, RoleId, ScreenId, CanRead, CanWrite, CanEdit, CanDelete)
    SELECT 
        @UserId,
        @RoleId,
        JSON_VALUE(p.value, '$.ScreenId'),
        CAST(JSON_VALUE(p.value, '$.CanRead') AS BIT),
        CAST(JSON_VALUE(p.value, '$.CanWrite') AS BIT),
        CAST(JSON_VALUE(p.value, '$.CanEdit') AS BIT),
        CAST(JSON_VALUE(p.value, '$.CanDelete') AS BIT)
    FROM OPENJSON(@Permissionjson) AS p
    WHERE NOT EXISTS (
        SELECT 1
        FROM UserRolePermissions urp
        WHERE urp.UserId = @UserId 
          AND urp.RoleId = @RoleId 
          AND urp.ScreenId = JSON_VALUE(p.value, '$.ScreenId')
    );
END;