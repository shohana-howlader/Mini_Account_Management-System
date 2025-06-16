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

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            //var users = await _context.Users
            // .Include(u => u.Role)
            // .ToListAsync();

            var users = await _context.Users
        .FromSqlRaw("EXEC sp_GetAllUsers_N")
        .ToListAsync();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName");
            return View();
        }

        // POST: Users/Create using Stored Procedure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password,RoleId")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Use stored procedure for insert
                    var parameters = new[]
                    {
                        new SqlParameter("@UserName", user.UserName),
                        new SqlParameter("@Password", user.Password), // Hash password in production
                        new SqlParameter("@CreatedDate", DateTime.Now)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_InsertUser @UserName, @Password, @RoleId, @CreatedDate",
                        parameters);

                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating user: " + ex.Message);
                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id");
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles);
            return View(user);
        }

        // POST: Users/Edit/5 using Stored Procedure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,RoleId,CreatedDate")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Use stored procedure for update
                    var parameters = new[]
                    {
                        new SqlParameter("@Id", user.Id),
                        new SqlParameter("@UserName", user.UserName),
                        new SqlParameter("@Password", user.Password)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_UpdateUser @Id, @UserName, @Password, @RoleId",
                        parameters);

                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating user: " + ex.Message);
                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id");
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5 using Stored Procedure
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Use stored procedure for delete
                var parameter = new SqlParameter("@Id", id);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteUser @Id",
                    parameter);

                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting user: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
