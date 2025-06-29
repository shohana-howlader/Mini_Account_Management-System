using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetConnectionString();
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await GetAllUsersAsync();
            return View(users);
        }


        // Add this new method to your existing controller

        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await GetAllUsersAsync();
                var userData = users.Select(u => new {
                    id = u.Id,
                    userName = u.UserName,
                    password = u.Password,
                    createdDate = u.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
                }).ToList();

                return Json(userData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { error = "Failed to load users" });
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [IgnoreAntiforgeryToken] // remove if you implement antiforgery tokens properly
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "Username and Password are required." });
            }

            try
            {
                var createdUser = await InsertUserAsync(user);

                if (createdUser != null && createdUser.Id > 0)
                {
                    return Ok(new { message = "User created successfully!" });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to create user." });
                }

            }
            catch (Exception ex)
            {
                // log exception here

                return StatusCode(500, new { message = "Server error: " + ex.Message });
            }
           
        }

        
        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedUser = await UpdateUserAsync(user);
                if (updatedUser != null)
                {
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "User not found or failed to update.");
                }
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await DeleteUserAsync(id);
            if (result > 0)
            {
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "User not found or failed to delete.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Private methods for stored procedure calls

        private async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32("Id"),
                                UserName = reader.GetString("UserName"),
                                Password = reader.GetString("Password"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            });
                        }
                    }
                }
            }

            return users;
        }

        private async Task<User> GetUserByIdAsync(int id)
        {
            User user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetUserById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("Id"),
                                UserName = reader.GetString("UserName"),
                                Password = reader.GetString("Password"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            };
                        }
                    }
                }
            }

            return user;
        }

        private async Task<User> InsertUserAsync(User user)
        {
            User createdUser = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_InsertUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@Password", user.Password);

                if (user.CreatedDate != default(DateTime))
                {
                    command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);
                }

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        createdUser = new User
                        {
                            Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                            UserName = reader["UserName"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                            CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.MinValue
                        };
                    }
                }
            }

            return createdUser;
        }


        private async Task<User> UpdateUserAsync(User user)
        {
            User updatedUser = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            updatedUser = new User
                            {
                                Id = reader.GetInt32("Id"),
                                UserName = reader.GetString("UserName"),
                                Password = reader.GetString("Password"),
                                CreatedDate = reader.GetDateTime("CreatedDate")
                            };
                        }
                    }
                }
            }

            return updatedUser;
        }

        private async Task<int> DeleteUserAsync(int id)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            rowsAffected = reader.GetInt32("RowsAffected");
                        }
                    }
                }
            }

            return rowsAffected;
        }
    }
}