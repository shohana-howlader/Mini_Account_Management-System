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
                    new SqlParameter("@CreatedDate", DateTime.Now)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_InsertRole @RoleName, @CreatedDate", parameters);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var roles = await _context.Roles
                .FromSqlRaw("EXEC sp_GetRoles")
                .ToListAsync();

            var role = roles.FirstOrDefault(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role)
        {
            if (id != role.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", role.Id),
                    new SqlParameter("@RoleName", role.RoleName),
                    new SqlParameter("@CreatedDate", role.CreatedDate)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_UpdateRole @Id, @RoleName, @CreatedDate", parameters);
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var parameter = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteRole @Id", parameter);
            return RedirectToAction(nameof(Index));
        }

    }
}
