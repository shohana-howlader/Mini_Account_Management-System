using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Models.ViewModel;
using System.Data;

namespace Mini_Account_Management_System.Controllers
{
    public class UserRolePermissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRolePermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserRolePermission
        public async Task<IActionResult> Index()
        {
            var permissions = new List<UserRolePermissionViewModel>();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_GetAllUserRolePermissions";
                    command.CommandType = CommandType.StoredProcedure;

                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(new UserRolePermissionViewModel
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                RoleId = reader.GetInt32("RoleId"),
                                ScreenId = reader.GetInt32("ScreenId"),
                                CanRead = reader.GetBoolean("CanRead"),
                                CanWrite = reader.GetBoolean("CanWrite"),
                                CanEdit = reader.GetBoolean("CanEdit"),
                                CanDelete = reader.GetBoolean("CanDelete"),
                                UserName = reader.GetString("UserName"),
                                RoleName = reader.GetString("RoleName"),
                                ScreenName = reader.GetString("ScreenName"),
                                URL = reader.IsDBNull("URL") ? "" : reader.GetString("URL")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["Error"] = $"Error loading permissions: {ex.Message}";
                return View(new List<UserRolePermissionViewModel>());
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return View(permissions);
        }

        // GET: UserRolePermission/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var permission = await GetPermissionById(id);
            if (permission == null)
            {
                return NotFound();
            }
            return View(permission);
        }

        // GET: UserRolePermission/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();

            var screens = await _context.Screens.ToListAsync();

            var model = new CreateUserRolePermissionViewModel
            {
                UserId = 0,
                RoleId = 0,
                Permissions = screens.Select(s => new UserRolePermissionViewModel
                {
                    ScreenId = s.Id,
                    ScreenName = s.ScreenName,
                    CanRead = false,
                    CanWrite = false,
                    CanEdit = false,
                    CanDelete = false
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRolePermissionViewModel model)
        {
            var logMessages = new List<string>();
            logMessages.Add($"=== CREATE ACTION STARTED ===");
            logMessages.Add($"UserId: {model.UserId}");
            logMessages.Add($"RoleId: {model.RoleId}");
            logMessages.Add($"Permissions Count: {model.Permissions?.Count ?? 0}");

            if (model.Permissions != null)
            {
                for (int i = 0; i < model.Permissions.Count; i++)
                {
                    var perm = model.Permissions[i];
                    logMessages.Add($"Permission[{i}] - ScreenId: {perm.ScreenId}, ScreenName: '{perm.ScreenName}', " +
                                  $"Read: {perm.CanRead}, Write: {perm.CanWrite}, Edit: {perm.CanEdit}, Delete: {perm.CanDelete}");
                }
            }
            else
            {
                logMessages.Add("WARNING: model.Permissions is NULL!");
            }

            foreach (var msg in logMessages)
            {
                System.Diagnostics.Debug.WriteLine(msg);
                Console.WriteLine(msg);
            }

            if (!ModelState.IsValid)
            {
                logMessages.Add("=== MODEL STATE INVALID ===");
                foreach (var error in ModelState)
                {
                    logMessages.Add($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                foreach (var msg in logMessages)
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                    Console.WriteLine(msg);
                }

                await PopulateDropDowns();
                await RepopulatePermissions(model);
                return View(model);
            }

            if (model.Permissions == null || !model.Permissions.Any())
            {
                logMessages.Add("ERROR: No permissions data received!");
                TempData["Error"] = "No permissions data received. Please try again.";
                await PopulateDropDowns();
                await RepopulatePermissions(model);
                return View(model);
            }

            try
            {
                // Filter permissions that have at least one permission granted
                var validPermissions = model.Permissions
                    .Where(p => p.CanRead || p.CanWrite || p.CanEdit || p.CanDelete)
                    .ToList();

                if (!validPermissions.Any())
                {
                    TempData["Warning"] = "No permissions were selected to save.";
                    await PopulateDropDowns();
                    await RepopulatePermissions(model);
                    return View(model);
                }

                logMessages.Add($"Processing {validPermissions.Count} valid permissions");

                // Create DataTable for Table-Valued Parameter
                var permissionsTable = new DataTable();
                permissionsTable.Columns.Add("ScreenId", typeof(int));
                permissionsTable.Columns.Add("CanRead", typeof(bool));
                permissionsTable.Columns.Add("CanWrite", typeof(bool));
                permissionsTable.Columns.Add("CanEdit", typeof(bool));
                permissionsTable.Columns.Add("CanDelete", typeof(bool));

                foreach (var permission in validPermissions)
                {
                    permissionsTable.Rows.Add(
                        permission.ScreenId,
                        permission.CanRead,
                        permission.CanWrite,
                        permission.CanEdit,
                        permission.CanDelete
                    );
                }

                logMessages.Add($"DataTable created with {permissionsTable.Rows.Count} rows");

                // Call the stored procedure with Table-Valued Parameter
                var results = new List<dynamic>();
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_InsertUserRolePermission_TVP";
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        var userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Value = model.UserId };
                        var roleIdParam = new SqlParameter("@RoleId", SqlDbType.Int) { Value = model.RoleId };
                        var permissionsParam = new SqlParameter("@Permissions", SqlDbType.Structured)
                        {
                            TypeName = "UserRolePermissionTableType",
                            Value = permissionsTable
                        };

                        command.Parameters.Add(userIdParam);
                        command.Parameters.Add(roleIdParam);
                        command.Parameters.Add(permissionsParam);

                        logMessages.Add("Executing stored procedure with Table-Valued Parameter");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new
                                {
                                    ScreenId = reader.GetInt32("ScreenId"),
                                    Action = reader.GetString("Action"),
                                    PermissionId = reader.GetInt32("PermissionId")
                                });
                            }
                        }
                    }
                }

                // Process results
                var insertedCount = results.Count(r => r.Action == "Inserted");
                var updatedCount = results.Count(r => r.Action == "Updated");
                var totalProcessed = results.Count;

                logMessages.Add($"=== PROCESSING COMPLETE ===");
                logMessages.Add($"Total Processed: {totalProcessed}");
                logMessages.Add($"Inserted: {insertedCount}");
                logMessages.Add($"Updated: {updatedCount}");

                foreach (var msg in logMessages)
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                    Console.WriteLine(msg);
                }

                if (totalProcessed > 0)
                {
                    var message = $"Successfully processed {totalProcessed} permissions! ";
                    if (insertedCount > 0 && updatedCount > 0)
                    {
                        message += $"({insertedCount} new, {updatedCount} updated)";
                    }
                    else if (insertedCount > 0)
                    {
                        message += $"({insertedCount} new permissions created)";
                    }
                    else if (updatedCount > 0)
                    {
                        message += $"({updatedCount} permissions updated)";
                    }

                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Warning"] = "No permissions were processed.";
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error processing permissions: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" Inner Exception: {ex.InnerException.Message}";
                }

                logMessages.Add($"CRITICAL ERROR: {errorMsg}");
                logMessages.Add($"Stack Trace: {ex.StackTrace}");

                foreach (var msg in logMessages)
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                    Console.WriteLine(msg);
                }

                TempData["Error"] = errorMsg;
            }

            // If we reach here, something went wrong
            await PopulateDropDowns();
            await RepopulatePermissions(model);
            return View(model);
        }

        // Helper method to repopulate permissions
        private async Task RepopulatePermissions(CreateUserRolePermissionViewModel model)
        {
            var allScreens = await _context.Screens.ToListAsync();
            model.Permissions = allScreens.Select(s => new UserRolePermissionViewModel
            {
                ScreenId = s.Id,
                ScreenName = s.ScreenName,
                CanRead = model.Permissions?.FirstOrDefault(p => p.ScreenId == s.Id)?.CanRead ?? false,
                CanWrite = model.Permissions?.FirstOrDefault(p => p.ScreenId == s.Id)?.CanWrite ?? false,
                CanEdit = model.Permissions?.FirstOrDefault(p => p.ScreenId == s.Id)?.CanEdit ?? false,
                CanDelete = model.Permissions?.FirstOrDefault(p => p.ScreenId == s.Id)?.CanDelete ?? false
            }).ToList();
        }

        // GET: UserRolePermission/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var permission = await GetPermissionById(id);
            if (permission == null)
            {
                return NotFound();
            }

            var editModel = new EditUserRolePermissionViewModel
            {
                Id = permission.Id,
                UserId = permission.UserId,
                RoleId = permission.RoleId,
                ScreenId = permission.ScreenId,
                CanRead = permission.CanRead,
                CanWrite = permission.CanWrite,
                CanEdit = permission.CanEdit,
                CanDelete = permission.CanDelete,
                UserName = permission.UserName,
                RoleName = permission.RoleName,
                ScreenName = permission.ScreenName
            };

            return View(editModel);
        }

        // POST: UserRolePermission/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserRolePermissionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@Id", model.Id),
                        new SqlParameter("@CanRead", model.CanRead),
                        new SqlParameter("@CanWrite", model.CanWrite),
                        new SqlParameter("@CanEdit", model.CanEdit),
                        new SqlParameter("@CanDelete", model.CanDelete)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_UpdateUserRolePermission @Id, @CanRead, @CanWrite, @CanEdit, @CanDelete",
                        parameters);

                    TempData["Success"] = "User role permission updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating permission: {ex.Message}";
                }
            }

            return View(model);
        }




        // POST: UserRolePermission/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id)

        {
            try
            {
                var parameter = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteUserRolePermission @Id", parameter);

                TempData["Success"] = "User role permission deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting permission: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: UserRolePermission/UserPermissions/5
        public async Task<IActionResult> UserPermissions(int userId)
        {
            var permissions = new List<UserRolePermissionViewModel>();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_GetUserPermissions";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(new UserRolePermissionViewModel
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                RoleId = reader.GetInt32("RoleId"),
                                ScreenId = reader.GetInt32("ScreenId"),
                                CanRead = reader.GetBoolean("CanRead"),
                                CanWrite = reader.GetBoolean("CanWrite"),
                                CanEdit = reader.GetBoolean("CanEdit"),
                                CanDelete = reader.GetBoolean("CanDelete"),
                                UserName = reader.GetString("UserName"),
                                RoleName = reader.GetString("RoleName"),
                                ScreenName = reader.GetString("ScreenName"),
                                URL = reader.IsDBNull("URL") ? "" : reader.GetString("URL")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading user permissions: {ex.Message}";
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            ViewBag.UserName = permissions.FirstOrDefault()?.UserName ?? "Unknown User";
            return View(permissions);
        }

        // GET: UserRolePermission/RolePermissions/5
        public async Task<IActionResult> RolePermissions(int roleId)
        {
            var permissions = new List<UserRolePermissionViewModel>();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_GetRolePermissions";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@RoleId", roleId));

                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(new UserRolePermissionViewModel
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                RoleId = reader.GetInt32("RoleId"),
                                ScreenId = reader.GetInt32("ScreenId"),
                                CanRead = reader.GetBoolean("CanRead"),
                                CanWrite = reader.GetBoolean("CanWrite"),
                                CanEdit = reader.GetBoolean("CanEdit"),
                                CanDelete = reader.GetBoolean("CanDelete"),
                                UserName = reader.GetString("UserName"),
                                RoleName = reader.GetString("RoleName"),
                                ScreenName = reader.GetString("ScreenName"),
                                URL = reader.IsDBNull("URL") ? "" : reader.GetString("URL")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading role permissions: {ex.Message}";
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            ViewBag.RoleName = permissions.FirstOrDefault()?.RoleName ?? "Unknown Role";
            return View(permissions);
        }

        // API: Check User Permission
        [HttpGet]
        public async Task<IActionResult> CheckPermission(int userId, int screenId, string permissionType)
        {
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_CheckUserScreenPermission";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserId", userId));
                    command.Parameters.Add(new SqlParameter("@ScreenId", screenId));
                    command.Parameters.Add(new SqlParameter("@PermissionType", permissionType));

                    await _context.Database.OpenConnectionAsync();
                    var result = await command.ExecuteScalarAsync();
                    bool hasPermission = Convert.ToBoolean(result);

                    return Json(new { hasPermission = hasPermission });
                }
            }
            catch (Exception ex)
            {
                return Json(new { hasPermission = false, error = ex.Message });
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        private async Task<UserRolePermissionViewModel> GetPermissionById(int id)
        {
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_GetAllUserRolePermissions";
                    command.CommandType = CommandType.StoredProcedure;

                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.GetInt32("Id") == id)
                            {
                                return new UserRolePermissionViewModel
                                {
                                    Id = reader.GetInt32("Id"),
                                    UserId = reader.GetInt32("UserId"),
                                    RoleId = reader.GetInt32("RoleId"),
                                    ScreenId = reader.GetInt32("ScreenId"),
                                    CanRead = reader.GetBoolean("CanRead"),
                                    CanWrite = reader.GetBoolean("CanWrite"),
                                    CanEdit = reader.GetBoolean("CanEdit"),
                                    CanDelete = reader.GetBoolean("CanDelete"),
                                    UserName = reader.GetString("UserName"),
                                    RoleName = reader.GetString("RoleName"),
                                    ScreenName = reader.GetString("ScreenName"),
                                    URL = reader.IsDBNull("URL") ? "" : reader.GetString("URL")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permission by ID: {ex.Message}");
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return null;
        }

        private async Task PopulateDropDowns()
        {
            try
            {
                ViewBag.Users = await _context.Users.Select(u => new { u.Id, u.UserName }).ToListAsync();
                ViewBag.Roles = await _context.Roles.Select(r => new { r.Id, r.RoleName }).ToListAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Users = new List<object>();
                ViewBag.Roles = new List<object>();
                TempData["Error"] = $"Error loading dropdown data: {ex.Message}";
            }
        }
    }
}