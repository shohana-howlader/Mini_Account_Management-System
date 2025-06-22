using System.Data;
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
            var viewModel = new UserRoleMappingPageViewModel
            {
                NewMapping = new UserRoleMappingViewModel(), // ✅ Needed for the form
                Users = await _context.Users.ToListAsync(),
                Roles = await _context.Roles.ToListAsync(),
                Mappings = new List<UserRoleMappingViewModel>()
            };

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_GetAllUserRoleMappings";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            viewModel.Mappings.Add(new UserRoleMappingViewModel
                            {
                                Id = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                RoleName = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return View(viewModel);
        }



        // GET: /UserRoleMappings/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRoleMappingPageViewModel viewModel)
        {
            var model = viewModel.NewMapping;

            Console.WriteLine($"UserId: {model.UserId}, RoleId: {model.RoleId}");

            var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == model.RoleId);

            if (!userExists || !roleExists)
            {
                ModelState.AddModelError("", "Invalid User or Role selection.");

                viewModel.Users = await _context.Users.ToListAsync();
                viewModel.Roles = await _context.Roles.ToListAsync();
                viewModel.Mappings = await LoadMappingsFromSP();

                return View("Index", viewModel);
            }

            var parameters = new[]
            {
        new SqlParameter("@UserId", model.UserId),
        new SqlParameter("@RoleId", model.RoleId)
    };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_InsertUserRoleMapping @UserId, @RoleId", parameters);

            return RedirectToAction(nameof(Index));
        }



        private async Task<List<UserRoleMappingViewModel>> LoadMappingsFromSP()
        {
            var mappings = new List<UserRoleMappingViewModel>();
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetAllUserRoleMappings";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                mappings.Add(new UserRoleMappingViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    UserName = reader.GetString(1),
                                    RoleName = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mappings: {ex.Message}");
            }

            return mappings;
        }




        // GET: /UserRoleMappings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var mapping = new UserRoleMappingViewModel();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_GetUserRoleMappingById";
                    command.CommandType = CommandType.StoredProcedure;

                    var param = new SqlParameter("@Id", id);
                    command.Parameters.Add(param);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            mapping.Id = reader.GetInt32(0);
                            mapping.UserName = reader.GetString(1);
                            mapping.RoleName = reader.GetString(2);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            return View(mapping);
        }




        // GET: /UserRoleMappings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var param = new SqlParameter("@Id", id);

            // ✅ Move FirstOrDefault to in-memory by using AsEnumerable
            var data = (await _context.Set<UserRoleMappingViewModel>()
                .FromSqlRaw("EXEC sp_GetUserRoleMappingById @Id", param)
                .ToListAsync())
                .FirstOrDefault();

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
