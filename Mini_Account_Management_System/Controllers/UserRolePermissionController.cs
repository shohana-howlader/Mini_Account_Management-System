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

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetAllUserRolePermissions";
                command.CommandType = CommandType.StoredProcedure;

                _context.Database.OpenConnection();
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
                            URL = reader.GetString("URL")
                        });
                    }
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
            return View();
        }

        // POST: UserRolePermission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRolePermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@RoleId", model.RoleId),
                    new SqlParameter("@ScreenId", model.ScreenId),
                    new SqlParameter("@CanRead", model.CanRead),
                    new SqlParameter("@CanWrite", model.CanWrite),
                    new SqlParameter("@CanEdit", model.CanEdit),
                    new SqlParameter("@CanDelete", model.CanDelete)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_InsertUserRolePermission @UserId, @RoleId, @ScreenId, @CanRead, @CanWrite, @CanEdit, @CanDelete",
                    parameters);

                TempData["Success"] = "User role permission created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns();
            return View(model);
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

            return View(model);
        }

        // GET: UserRolePermission/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var permission = await GetPermissionById(id);
            if (permission == null)
            {
                return NotFound();
            }
            return View(permission);
        }

        // POST: UserRolePermission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parameter = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteUserRolePermission @Id", parameter);

            TempData["Success"] = "User role permission deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserRolePermission/UserPermissions/5
        public async Task<IActionResult> UserPermissions(int userId)
        {
            var permissions = new List<UserRolePermissionViewModel>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetUserPermissions";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", userId));

                _context.Database.OpenConnection();
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
                            URL = reader.GetString("URL")
                        });
                    }
                }
            }

            ViewBag.UserName = permissions.FirstOrDefault()?.UserName ?? "Unknown User";
            return View(permissions);
        }

        // GET: UserRolePermission/RolePermissions/5
        public async Task<IActionResult> RolePermissions(int roleId)
        {
            var permissions = new List<UserRolePermissionViewModel>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetRolePermissions";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@RoleId", roleId));

                _context.Database.OpenConnection();
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
                            URL = reader.GetString("URL")
                        });
                    }
                }
            }

            ViewBag.RoleName = permissions.FirstOrDefault()?.RoleName ?? "Unknown Role";
            return View(permissions);
        }

        // API: Check User Permission
        [HttpGet]
        public async Task<IActionResult> CheckPermission(int userId, int screenId, string permissionType)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_CheckUserScreenPermission";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", userId));
                command.Parameters.Add(new SqlParameter("@ScreenId", screenId));
                command.Parameters.Add(new SqlParameter("@PermissionType", permissionType));

                _context.Database.OpenConnection();
                var result = await command.ExecuteScalarAsync();
                bool hasPermission = Convert.ToBoolean(result);

                return Json(new { hasPermission = hasPermission });
            }
        }

        // GET: Dashboard Data
        public async Task<IActionResult> Dashboard()
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetDashboardData";
                command.CommandType = CommandType.StoredProcedure;

                _context.Database.OpenConnection();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var dashboardData = new DashboardViewModel
                        {
                            TotalUsers = reader.GetInt32("TotalUsers"),
                            TotalRoles = reader.GetInt32("TotalRoles"),
                            TotalScreens = reader.GetInt32("TotalScreens"),
                            TotalPermissions = reader.GetInt32("TotalPermissions")
                        };
                        return View(dashboardData);
                    }
                }
            }

            return View(new DashboardViewModel());
        }

        private async Task<UserRolePermissionViewModel> GetPermissionById(int id)
        {
            var permissions = new List<UserRolePermissionViewModel>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetAllUserRolePermissions";
                command.CommandType = CommandType.StoredProcedure;

                _context.Database.OpenConnection();
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
                                URL = reader.GetString("URL")
                            };
                        }
                    }
                }
            }

            return null;
        }

        private async Task PopulateDropDowns()
        {
            ViewBag.Users = await _context.Users.Select(u => new { u.Id, u.UserName }).ToListAsync();
            ViewBag.Roles = await _context.Roles.Select(r => new { r.Id, r.RoleName }).ToListAsync();
            ViewBag.Screens = await _context.Screens.Select(s => new { s.Id, s.ScreenName }).ToListAsync();
        }
    }
}
