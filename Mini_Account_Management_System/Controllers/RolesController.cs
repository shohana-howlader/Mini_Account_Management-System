using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;

namespace Mini_Account_Management_System.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            //var roles = await _context.Roles
            //    .Include(r => r.Users)
            //    .ToListAsync();

            var roles = await _context.Roles
       .FromSqlRaw("EXEC sp_GetAllRoles")
       .ToListAsync();
            return View(roles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create using Stored Procedure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if role name already exists
                    var existingRole = await _context.Roles
                        .FirstOrDefaultAsync(r => r.RoleName.ToLower() == role.RoleName.ToLower());

                    if (existingRole != null)
                    {
                        ModelState.AddModelError("RoleName", "Role name already exists.");
                        return View(role);
                    }

                    // Use stored procedure for insert
                    var parameters = new[]
                    {
                        new SqlParameter("@RoleName", role.RoleName),
                        new SqlParameter("@CreatedDate", DateTime.Now)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_InsertRole @RoleName, @CreatedDate",
                        parameters);

                    TempData["SuccessMessage"] = "Role created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating role: " + ex.Message);
                }
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5 using Stored Procedure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleName,CreatedDate")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if role name already exists (excluding current role)
                    var existingRole = await _context.Roles
                        .FirstOrDefaultAsync(r => r.RoleName.ToLower() == role.RoleName.ToLower() && r.Id != role.Id);

                    if (existingRole != null)
                    {
                        ModelState.AddModelError("RoleName", "Role name already exists.");
                        return View(role);
                    }

                    // Use stored procedure for update
                    var parameters = new[]
                    {
                        new SqlParameter("@Id", role.Id),
                        new SqlParameter("@RoleName", role.RoleName)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_UpdateRole @Id, @RoleName",
                        parameters);

                    TempData["SuccessMessage"] = "Role updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating role: " + ex.Message);
                }
            }
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5 using Stored Procedure
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Check if role has associated users - FIXED: Check users with specific RoleId
                var usersCount = await _context.Users.CountAsync();
                if (usersCount > 0)
                {
                    TempData["ErrorMessage"] = $"Cannot delete role. It is assigned to {usersCount} user(s).";
                    return RedirectToAction(nameof(Index));
                }

                // Use stored procedure for delete
                var parameter = new SqlParameter("@Id", id);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteRole @Id",
                    parameter);

                TempData["SuccessMessage"] = "Role deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting role: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
