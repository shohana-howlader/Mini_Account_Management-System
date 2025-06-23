using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Models.ViewModel;

namespace Mini_Account_Management_System.Controllers
{
    public class UserRolePermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolePermissionController> _logger;

        public UserRolePermissionController(ApplicationDbContext context, ILogger<UserRolePermissionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Create View
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Users = await _context.Users.OrderBy(u => u.UserName).ToListAsync();
                ViewBag.Roles = await _context.Roles.OrderBy(r => r.RoleName).ToListAsync();
                ViewBag.Screens = await _context.Screens.OrderBy(s => s.ScreenName).ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create view");
                ViewBag.ErrorMessage = "Error loading data. Please try again.";
                return View();
            }
        }

        // GET: Index/List View
        public async Task<IActionResult> Index()
        {
            try
            {
                var permissions = await _context.UserRolePermissions
                    .Include(p => p.User)
                    .Include(p => p.Role)
                    .Include(p => p.Screen)
                    .OrderBy(p => p.User.UserName)
                    .ThenBy(p => p.Role.RoleName)
                    .ThenBy(p => p.Screen.ScreenName)
                    .ToListAsync();

                return View(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading permissions list");
                ViewBag.ErrorMessage = "Error loading permissions. Please try again.";
                return View(new List<UserRolePermission>());
            }
        }

        // POST: Get Existing Permissions
        [HttpPost]
        public async Task<IActionResult> GetPermissions([FromBody] GetPermissionsRequest request)
        {
            try
            {
                if (request == null || (request.UserId == null && request.RoleId == null))
                {
                    return BadRequest(new { message = "Either UserId or RoleId must be provided" });
                }

                var parameters = new[]
                {
                    new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value),
                    new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value)
                };

                // Use stored procedure to get permissions with joins
                var query = @"
                    DECLARE @temp TABLE (
                        Id INT,
                        UserId INT,
                        UserName NVARCHAR(255),
                        RoleId INT,
                        RoleName NVARCHAR(255),
                        ScreenId INT,
                        ScreenName NVARCHAR(255),
                        CanRead BIT,
                        CanWrite BIT,
                        CanEdit BIT,
                        CanDelete BIT
                    );
                    
                    INSERT INTO @temp
                    EXEC sp_GetUserRolePermissions @UserId, @RoleId;
                    
                    SELECT * FROM @temp;
                ";

                var permissionsData = new List<dynamic>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value));

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissionsData.Add(new
                            {
                                id = reader.GetInt32("Id"),
                                userId = reader.IsDBNull("UserId") ? (int?)null : reader.GetInt32("UserId"),
                                userName = reader.IsDBNull("UserName") ? "" : reader.GetString("UserName"),
                                roleId = reader.IsDBNull("RoleId") ? (int?)null : reader.GetInt32("RoleId"),
                                roleName = reader.IsDBNull("RoleName") ? "" : reader.GetString("RoleName"),
                                screenId = reader.GetInt32("ScreenId"),
                                screenName = reader.IsDBNull("ScreenName") ? "" : reader.GetString("ScreenName"),
                                canRead = reader.GetBoolean("CanRead"),
                                canWrite = reader.GetBoolean("CanWrite"),
                                canEdit = reader.GetBoolean("CanEdit"),
                                canDelete = reader.GetBoolean("CanDelete")
                            });
                        }
                    }
                }

                return Json(permissionsData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for UserId: {UserId}, RoleId: {RoleId}",
                    request?.UserId, request?.RoleId);
                return StatusCode(500, new { message = "Error loading permissions: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute
        public async Task<IActionResult> SavePermissions([FromBody] CreateUserRolePermissionViewModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("SavePermissions called with null model");
                    return BadRequest(new { message = "Request body is null" });
                }

                _logger.LogInformation("SavePermissions called with UserId: {UserId}, RoleId: {RoleId}, Permissions count: {Count}",
                    model.UserId, model.RoleId, model.Permissions?.Count ?? 0);

                // Check if both UserId and RoleId are null/0
                if ((!model.UserId.HasValue || model.UserId == 0) && (!model.RoleId.HasValue || model.RoleId == 0))
                {
                    return BadRequest(new { message = "Either UserId or RoleId must be provided and greater than 0." });
                }

                if (model.Permissions == null || !model.Permissions.Any())
                {
                    return BadRequest(new { message = "Permissions are required." });
                }

                // Convert UserId and RoleId to proper values for database
                var userIdParam = model.UserId.HasValue && model.UserId > 0 ? model.UserId.Value : (object)DBNull.Value;
                var roleIdParam = model.RoleId.HasValue && model.RoleId > 0 ? model.RoleId.Value : (object)DBNull.Value;

                var permissionJson = JsonSerializer.Serialize(model.Permissions);
                _logger.LogInformation("Serialized permissions JSON: {Json}", permissionJson);

                var parameters = new[]
                {
            new SqlParameter("@UserId", userIdParam),
            new SqlParameter("@RoleId", roleIdParam),
            new SqlParameter("@Permissionjson", permissionJson)
        };

                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_InsertMultipleUserRolePermissions @UserId, @RoleId, @Permissionjson",
                    parameters
                );

                _logger.LogInformation("Stored procedure executed successfully. Result: {Result}", result);

                return Ok(new { message = "Permissions saved successfully" });
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while saving permissions");
                return StatusCode(500, new { message = "Database error occurred while saving permissions", error = sqlEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving permissions");
                return StatusCode(500, new { message = "An error occurred while saving permissions", error = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeletePermissions([FromBody] DeletePermissionsRequest request)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value),
                    new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value),
                    new SqlParameter("@ScreenId", request.ScreenId ?? (object)DBNull.Value)
                };

                var result = new List<dynamic>();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_DeleteUserRolePermissions @UserId, @RoleId, @ScreenId";
                    command.Parameters.Add(new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@ScreenId", request.ScreenId ?? (object)DBNull.Value));

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var rowsDeleted = reader.GetInt32("RowsDeleted");
                            return Ok(new { message = $"{rowsDeleted} permission(s) deleted successfully!" });
                        }
                    }
                }

                return Ok(new { message = "Permissions deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permissions");
                return StatusCode(500, new { message = "Error deleting permissions: " + ex.Message });
            }
        }

        // GET: Get All Screens for Permission Setup
        [HttpGet]
        public async Task<IActionResult> GetAllScreens()
        {
            try
            {
                var screens = new List<dynamic>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_GetAllScreensForPermission";

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            screens.Add(new
                            {
                                id = reader.GetInt32("Id"),
                                screenName = reader.GetString("ScreenName"),
                                Url = reader.IsDBNull("Url") ? "" : reader.GetString("Url"),
                                isActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }

                return Json(screens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all screens");
                return StatusCode(500, new { message = "Error loading screens: " + ex.Message });
            }
        }

        // POST: Clear Permissions for User or Role
        [HttpPost]
        public async Task<IActionResult> ClearPermissions([FromBody] ClearPermissionsRequest request)
        {
            try
            {
                if (request == null || (request.UserId == null && request.RoleId == null))
                {
                    return BadRequest(new { message = "Either UserId or RoleId must be provided" });
                }

                var parameters = new[]
                {
                    new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value),
                    new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value),
                    new SqlParameter("@ScreenId", (object)DBNull.Value)
                };

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_DeleteUserRolePermissions @UserId, @RoleId, @ScreenId";
                    command.Parameters.Add(new SqlParameter("@UserId", request.UserId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@RoleId", request.RoleId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@ScreenId", (object)DBNull.Value));

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var rowsDeleted = reader.GetInt32("RowsDeleted");
                            return Ok(new { message = $"{rowsDeleted} permission(s) cleared successfully!" });
                        }
                    }
                }

                return Ok(new { message = "Permissions cleared successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing permissions");
                return StatusCode(500, new { message = "Error clearing permissions: " + ex.Message });
            }
        }
    }

    // DTO Classes
    public class GetPermissionsRequest
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }

   

    

    public class DeletePermissionsRequest
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? ScreenId { get; set; }
    }

    public class ClearPermissionsRequest
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}