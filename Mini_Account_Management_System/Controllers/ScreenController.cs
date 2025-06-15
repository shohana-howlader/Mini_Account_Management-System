using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models;
using System.Data;

namespace Mini_Account_Management_System.Controllers
{
    public class ScreenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ScreenController(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        // GET: Screen
        public async Task<IActionResult> Index()
        {
            var screens = new List<Screen>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllScreens", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            screens.Add(new Screen
                            {
                                Id = reader.GetInt32("Id"),
                                ScreenName = reader.GetString("ScreenName"),
                                URL = reader.IsDBNull("URL") ? null : reader.GetString("URL")
                            });
                        }
                    }
                }
            }

            return View(screens);
        }

        // GET: Screen/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var screen = await GetScreenByIdAsync(id);
            if (screen == null)
            {
                return NotFound();
            }

            return View(screen);
        }

        // GET: Screen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Screen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScreenName,URL")] Screen screen)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("sp_InsertScreen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ScreenName", screen.ScreenName);
                        command.Parameters.AddWithValue("@URL", screen.URL ?? (object)DBNull.Value);

                        await connection.OpenAsync();
                        var result = await command.ExecuteScalarAsync();
                        screen.Id = Convert.ToInt32(result);
                    }
                }

                TempData["SuccessMessage"] = "Screen created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(screen);
        }

        // GET: Screen/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var screen = await GetScreenByIdAsync(id);
            if (screen == null)
            {
                return NotFound();
            }

            return View(screen);
        }

        // POST: Screen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScreenName,URL")] Screen screen)
        {
            if (id != screen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("sp_UpdateScreen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", screen.Id);
                        command.Parameters.AddWithValue("@ScreenName", screen.ScreenName);
                        command.Parameters.AddWithValue("@URL", screen.URL ?? (object)DBNull.Value);

                        await connection.OpenAsync();
                        var rowsAffected = await command.ExecuteScalarAsync();

                        if (Convert.ToInt32(rowsAffected) == 0)
                        {
                            return NotFound();
                        }
                    }
                }

                TempData["SuccessMessage"] = "Screen updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(screen);
        }

        // GET: Screen/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var screen = await GetScreenByIdAsync(id);
            if (screen == null)
            {
                return NotFound();
            }

            return View(screen);
        }

        // POST: Screen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteScreen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteScalarAsync();

                    if (Convert.ToInt32(rowsAffected) == 0)
                    {
                        TempData["ErrorMessage"] = "Screen not found or could not be deleted.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            TempData["SuccessMessage"] = "Screen deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<Screen> GetScreenByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, ScreenName, URL FROM Screens WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Screen
                            {
                                Id = reader.GetInt32("Id"),
                                ScreenName = reader.GetString("ScreenName"),
                                URL = reader.IsDBNull("URL") ? null : reader.GetString("URL")
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
