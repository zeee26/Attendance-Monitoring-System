using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZMAttendanceMonitoring.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
//using BCrypt.Net;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using EmployeeAttendanceApp.Models;
using System.Diagnostics;


namespace EmployeeAttendanceApp.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;

        }

        //Dashboard displaying today's attendance logs
        public IActionResult Index(int? page)
        {

            // ENFORCE LOGOUT: Redirect logged-in admins back to the History portal
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                TempData["AdminMessage"] = "🔒 You must log out of the admin session before going back to the home page.";
                return RedirectToAction(nameof(History));
            }
            // Defaults to /Home
            if (Request.Path.Value == "/")
            {
                return RedirectToAction(nameof(Index), new { controller = "Attendance" });
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int employeeId)
        {
            // Return an error for employeeId < 0
            if (employeeId <= 0)
            {
                TempData["Error"] = "Please enter a valid Employee ID.";
                return RedirectToAction(nameof(Index));
            }

            // Verify that the Employee ID actually exists in the system
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                TempData["Error"] = $"Employee ID {employeeId} not found in the records.";
                return RedirectToAction(nameof(Index));
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Prevent duplicate check-ins for the same day
            var existingRecord = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (existingRecord == null)
            {
                var record = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = today,
                    CheckInTime = TimeOnly.FromDateTime(DateTime.Now)
                };
                _context.Add(record);
                await _context.SaveChangesAsync();

                // Personalized success greeting using their retrieved profile name
                TempData["Message"] = $"Mabuhay {employee.FirstName}! Checked in successfully at {record.CheckInTime?.ToString("hh:mm tt")}.";
            }
            else
            {
                TempData["Error"] = $"{employee.FullName}, you have already checked in for today.";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int employeeId)
        {
            
            if (employeeId <= 0) return RedirectToAction(nameof(Index));

            // Verify that the Employee ID actually exists in the system
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                TempData["Error"] = $"Employee ID {employeeId} not found.";
                return RedirectToAction(nameof(Index));
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Find today's active shift for this specific ID that hasn't checked out yet
            var record = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today && a.CheckOutTime == null);

            if (record != null)
            {
                record.CheckOutTime = TimeOnly.FromDateTime(DateTime.Now);
                _context.Update(record);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Salamat {employee.FirstName}! Checked out successfully at {record.CheckOutTime?.ToString("hh:mm tt")}.";
            }
            else
            {
                TempData["Error"] = $"No active check-in record found for {employee.FirstName} today.";
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Admin
        [Authorize]
        public async Task<IActionResult> History(DateOnly? filterDate, int? page)
        {
            // Pagination constraints
            int currentPage = page ?? 1;
            if (currentPage < 1) currentPage = 1;
            int pageSize = 10;

            var selectedDate = filterDate ?? DateOnly.FromDateTime(DateTime.Today);

            // Total records/page matching specific date
            int totalRecords = await _context.Attendances.CountAsync(a => a.Date == selectedDate);
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // FIXED: Added .Include(a => a.Employee) to load relational data
            var historyLogs = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date == selectedDate)
                //.OrderBy(a => a.Employee != null ? a.Employee.LastName : "") // Sort by Last Name
                .OrderBy(a => a.CheckInTime == null) // Puts nulls last
                .ThenBy(a => a.CheckInTime)          // Orders valid times chronologically
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Store the selected date in ViewData
            ViewData["SelectedDate"] = selectedDate.ToString("yyyy-MM-dd");
            ViewData["RawDate"] = selectedDate; // Kept as object for route binding
            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = totalPages;

            return View(historyLogs);
        }

        // GET: Admin/ExportToCsv
        [Authorize]
        public async Task<IActionResult> ExportToCsv(DateOnly? filterDate)
        {
            // Default to today if no date is specified
            var selectedDate = filterDate ?? DateOnly.FromDateTime(DateTime.Today);

            //// Fetch the data for the given date
            // FIXED: Added .Include(a => a.Employee)
            var records = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date == selectedDate)
                .OrderBy(a => a.CheckInTime == null) // Puts nulls last
                .ThenBy(a => a.CheckInTime)          // Orders valid times chronologically
                .ToListAsync();

            // Setup CSV formatting string builder
            var csvBuilder = new StringBuilder();

            // Write CSV Headers
            csvBuilder.AppendLine("Employee Id,Employee Name,Date,Check-In Time,Check-Out Time,Hours Worked");

            // Populate CSV Rows
            foreach (var record in records)
            {
                var employeeId = record.EmployeeId;
                var checkIn = record.CheckInTime?.ToString("hh:mm tt") ?? "N/A";
                var checkOut = record.CheckOutTime?.ToString("hh:mm tt") ?? "Active Shift";

                // Escape quotes/commas in names if necessary
                // FIXED: Using record.Employee.FullName safely with null-conditional checking
                var fullName = record.Employee != null ? record.Employee.FullName : "Unknown Employee";
                var escapedName = fullName.Contains(",") ? $"\"{fullName}\"" : fullName;

                // Append the row to the CSV
                csvBuilder.AppendLine($"{employeeId},{escapedName},{record.Date:yyyy-MM-dd},{checkIn},{checkOut},{record.TotalHoursWorked}");
            }

            // Convert string data to a UTF-8 Byte Array
            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Create a dynamic filename (e.g., Attendance_Report_2026-09-26.csv)
            string fileName = $"Attendance_Report_{selectedDate:yyyy-MM-dd}.csv";

            // Return the byte array as a downloadable CSV content-type
            return File(csvBytes, "text/csv", fileName);
        }

        // Admin Login screen
        [HttpGet]
        public IActionResult Login() => View();

        // Admin login process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string password)
        {
            var storedSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == "AdminPasswordHash");

            if (storedSetting != null && BCrypt.Net.BCrypt.Verify(password, storedSetting.SettingValue))
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction(nameof(History));
            }

            ModelState.AddModelError("", "Invalid Admin Password.");
            return View();
        }

        // Admin Logout action
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        // Admin change password screen
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        // Admin change password
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var storedSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == "AdminPasswordHash");

            if (storedSetting == null || !BCrypt.Net.BCrypt.Verify(currentPassword, storedSetting.SettingValue))
            {
                ModelState.AddModelError("", "Current password entry is incorrect.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("", "New password must be at least 6 characters long.");
                return View();
            }

            storedSetting.SettingValue = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Update(storedSetting);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin password updated successfully!";
            return RedirectToAction(nameof(History));
        }


        // GET: Attendance/AddEmployee
        [Authorize]
        [HttpGet]
        public IActionResult AddEmployee() => View();

        // POST: Attendance/AddEmployee
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();

                TempData["AdminMessage"] = $"Employee {model.FirstName} {model.LastName}, with ID {model.Id} added successfully!";
                return RedirectToAction(nameof(History));
            }
            return View(model);
        }

        public IActionResult PrivacyPolicy()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            // Returns the view layout framework
            //return View();
            return View(new PrivacyPolicyViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            // Returns the view layout framework
            //return View();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

}
