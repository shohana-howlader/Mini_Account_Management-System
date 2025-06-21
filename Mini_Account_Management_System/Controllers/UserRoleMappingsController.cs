using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Models.ViewModel;

namespace Mini_Account_Management_System.Controllers
{
    public class UserRoleMappingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRoleMappingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /UserRoleMappings
        public async Task<IActionResult> Index()
        {
            var userRoles = new List<UserRoleMappingViewModel>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_GetAllUserRoleMappings";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userRoles.Add(new UserRoleMappingViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                RoleName = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return View(userRoles);

        }

        // GET: /UserRoleMappings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var param = new SqlParameter("@Id", id);

            var data = await _context.Set<UserRoleMappingViewModel>()
                .FromSqlRaw("EXEC sp_GetUserRoleMappingById @Id", param)
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound();

            return View(data);
        }

        // GET: /UserRoleMappings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        // POST: /UserRoleMappings/Create
        [HttpPost]
        public async Task<IActionResult> Create(UserRoleMapping model)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", model.UserId),
                new SqlParameter("@RoleId", model.RoleId)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_InsertUserRoleMapping @UserId, @RoleId", parameters);
            return RedirectToAction(nameof(Index));
        }

        // GET: /UserRoleMappings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var param = new SqlParameter("@Id", id);

            var data = await _context.Set<UserRoleMappingViewModel>()
                .FromSqlRaw("EXEC sp_GetUserRoleMappingById @Id", param)
                .FirstOrDefaultAsync();

            if (data == null) return NotFound();

            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();

            var model = new UserRoleMapping
            {
                Id = data.Id,
                UserId = data.UserId,
                RoleId = data.RoleId
            };

            return View(model);
        }

        // POST: /UserRoleMappings/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleMapping model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@UserId", model.UserId),
                new SqlParameter("@RoleId", model.RoleId)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_UpdateUserRoleMapping @Id, @UserId, @RoleId", parameters);
            return RedirectToAction(nameof(Index));
        }

        // GET: /UserRoleMappings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var param = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteUserRoleMapping @Id", param);
            return RedirectToAction(nameof(Index));
        }
    }
}
