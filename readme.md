# ZM Attendance Monitoring System
# Published: http://zmattendancemonitoringsystem.runasp.net/Home

## 📄 Title
**ZMAttendanceMonitoring** — Modern, High-Performance Employee Attendance Logging Kiosk Terminal.

---

## 🎯 Purpose
The purpose of **ZMAttendanceMonitoring** is to replace outdated, manual, or slow paper-and-dropdown tracking methods with a fast, secure, standalone hardware-ready kiosk. It is engineered to process rapid workspace attendance inputs using numerical IDs or automated hardware barcode sweeps, minimizing bottleneck cues during shifts while maintaining absolute database data integrity.

---

## 📝 Overview or Description
**ZMAttendanceMonitoring** is a cross-platform, server-side rendered (SSR) web application built using the modern **.NET 8.0 ASP.NET Core MVC** framework and backed by a relational **SQL Server LocalDB** engine. 

### Core Engineering Highlights:
*   **Kiosk Optimization UI:** Features a client-side vanilla JavaScript real-time clock engine and a localized greeting system (*"Mabuhay! Magandang Araw!"*) running inside a premium glassmorphic Bootstrap 5 theme.
*   **Numerical & Barcode Entry Framework:** Replaced legacy select fields with a standard autofocusing text field optimized for numeric keystrokes and native, plug-and-play USB hardware barcode/QR code scanners.
*   **Code-First Relational Core:** Leverages Entity Framework Core 8.0 to govern a strict Parent-Child mapping structure between `Employees` and `Attendances`, automatically managing cascading rules.
*   **State-Based Cookie Authentication Gate:** Secures administrative access using lightweight data protection cookie middleware and cryptographic `BCrypt.Net` password hashing.
*   **System Settings Matrix:** Employs a dynamic key-value database table (`SystemSetting`) to decouple application configuration items (like passwords) from compiled source code.
*   **High-Throughput Performance Scaling:** Mitigates operational memory bloat via server-side dataset pagination partition streams (`.Skip()` and `.Take()`), rendering exactly 10 lines at a time.
*   **One-Click Financial Export Pipelines:** Compiles filtered daily attendance metrics into cleanly escaped, industry-standard CSV streams designed for direct ingestion into Excel or Google Sheets.

---

## 🛠️ Setup and Installation Instructions

### Prerequisites
*   [.NET 8.0 SDK](https://microsoft.com)
*   [SQL Server Express LocalDB](https://microsoft.com) (Included with Visual Studio Data Workloads)

### Local Configuration Steps

1.  **Clone and Navigate to the Repository Space:**
    ```bash
    git clone https://github.com
    cd ZMAttendanceMonitoring
    ```

2.  **Restore Missing NuGet Package Dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Execute Database Structural Initialization:**
    Apply the Code-First migration blueprints straight into your LocalDB instance:
    ```bash
    dotnet ef database update
    ```

4.  **Boot Up the Web Kiosk Host:**
    ```bash
    dotnet run
    ```
    *Open your browser and navigate to the application terminal path: `https://localhost:7xxxx/Home`*

> 🔒 **Default Credentials:** Initial Administrative password is **`Admin123`**. Update this immediately upon initial login via the security settings control tray.

---

## 💻 In-Practice Usage with Command-Line Commands or Code

### A. Database Management CLI Operations
If you need to perform database schema maintenance or safe row truncations via your PowerShell / Command Prompt terminal, utilize the following commands:

*   **View Live Employee Master Table Records:**
    ```powershell
    dotnet ef database execute-mmd "SELECT * FROM Employees;"
    ```
*   **Safely Delete Duplicate Entries While Keeping the Original Row ID:**
    ```powershell
    dotnet ef database execute-mmd "WITH CTE AS (SELECT Id, ROW_NUMBER() OVER (PARTITION BY FirstName, LastName ORDER BY Id ASC) as RowNum FROM Employees) DELETE FROM Employees WHERE Id IN (SELECT Id FROM CTE WHERE RowNum > 1);"
    ```

### B. Core Back-End Implementation Code
The application avoids over-engineered repository patterns, utilizing direct dependency injection to enforce the **KISS** (Keep It Simple, Stupid) design principle:

```csharp
// Location: Controllers/AttendanceController.cs
[Route("")]
public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _context;

    // Inversion of Control via Constructor Dependency Injection
    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Process Numerical ID Input Submissions
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Attendance/CheckIn")]
    public async Task<IActionResult> CheckIn(int employeeId)
    {
        if (employeeId < 0) return RedirectToAction(nameof(Index));

        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            TempData["Error"] = "❌ Employee ID not found.";
            return RedirectToAction(nameof(Index));
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var existing = await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

        if (existing == null)
        {
            var log = new Attendance { EmployeeId = employeeId, Date = today, CheckInTime = TimeOnly.FromDateTime(DateTime.Now) };
            _context.Add(log);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"📥 Mabuhay {employee.FirstName}! Checked in safely.";
        }
        return RedirectToAction(nameof(Index));
    }
}
```

### C. Advanced Global Route Mapping
Centralized conventional routing maps configured inside `Program.cs` handle clean URL generation without creating double-slash formatting anomalies:

```csharp
// Location: Program.cs
app.MapControllerRoute(
    name: "home_explicit",
    pattern: "Home",
    defaults: new { controller = "Attendance", action = "Index" });

// Admin
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Attendance", action = "History" });

// Error
app.MapControllerRoute(
    name: "error",
    pattern: "Error",
    //defaults: new { controller = "Home", action = "Error" });
    defaults: new { controller = "Attendance", action = "Error" });

// Privacy
app.MapControllerRoute(
    name: "privacy_policy",
    pattern: "PrivacyPolicy",
    //defaults: new { controller = "Home", action = "Error" });
    defaults: new { controller = "Attendance", action = "PrivacyPolicy" });

// Global default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Attendance}/{action=Index}/{id?}");
```

---

## 👥 Contributors
*   **Lead Software Architect & Developer:** Zarah Marie T. Moran
*   **AI Collaborator:** ChatGPT Architecture Engine

---

## 📄 Software License
This project is licensed under the **MIT License** - see the full description profile below:

```text
MIT License

Copyright (c) 2026 ZMAttendanceMonitoring Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
