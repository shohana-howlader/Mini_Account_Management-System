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
            var roles = await _context.Roles
                .FromSqlRaw("EXEC sp_GetRoles")
                .ToListAsync();
            return View(roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
                    new SqlParameter("@RoleName", role.RoleName),
                    new SqlParameter("@Description", (object?)role.Description ?? DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                };
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_InsertRole @RoleName, @Description, @CreatedDate", parameters);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await GetRoleByIdAsync(id);

            if (role == null)
                return NotFound();

            // Pass current date to view - only current date allowed
            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role)
        {
            if (id != role.Id)
                return NotFound();

            // Always override created date to now
            role.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
            new SqlParameter("@Id", role.Id),
            new SqlParameter("@RoleName", role.RoleName),
            new SqlParameter("@Description", (object?)role.Description ?? DBNull.Value),
            new SqlParameter("@CreatedDate", role.CreatedDate)
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_UpdateRole @Id, @RoleName, @Description, @CreatedDate", parameters);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View(role);
        }


        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = await GetRoleByIdAsync(id);

            if (role == null)
                return NotFound();

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var param = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteRole @Id", param);
            return RedirectToAction(nameof(Index));
        }

        // Helper method to get role by ID
        private async Task<Role?> GetRoleByIdAsync(int id)
        {
            var param = new SqlParameter("@Id", id);
            var roles = await _context.Roles
                .FromSqlRaw("EXEC sp_GetRoleById @Id", param)
                .ToListAsync();

            return roles.FirstOrDefault();
        }
    }
}