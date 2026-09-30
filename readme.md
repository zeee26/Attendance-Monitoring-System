# ZM ATTENDANCE MONITORING APP
** kiosk-style web application running on a dedicated company machine and only requires an employee ID
** Document HighlightsArchitecture & Tech Stack Summary
: Highlighting your move to .NET 8.0, Entity Framework Core, 
and LocalDB.Relational Schema Diagrams
: Outlining the clean parent-child foreign key relationship between the Employees and Attendances tables.
Technical Process Flow
: Step-by-step description of the data transmission from barcode scans to the backend.
Operations Guide
: Clear instructions for daily employee logging and admin payroll export functions.If you want to keep enhancing your deployment ecosystem, let me know if you would like

=========================================================================
=========================================================================
=========================================================================
=========================================================================

AdminPassword:
Admin123
Admin1234

=========================================================================

Package Manager Console:

Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.26
Add-Migration InitialCreate (To remove: Remove-Migration)
Update-Database							>>>Truncates the table
Add-Migration AddSystemSettings
Update-Database
dotnet add package BCrypt.Net-Next

=========================================================================
View > Terminal (Powershell):

To add table: Employees
# 1. Clear out all existing data using raw SQL via dotnet tool (keeps table structures intact)
dotnet ef database execute-mmd "DELETE FROM Attendances;"

# 2. Generate a new structural migration file for the Employee relation
dotnet ef migrations add AddEmployeesTable

# 3. Apply changes directly into your LocalDB instance
dotnet ef database update



=========================================================================
Wipe and Reset via Package Manager Console / PowerShell
# 1. Rollback the database state to the step right before the conflicting migration
dotnet ef database update AddSystemSettings

# 2. Delete the problematic migration file completely from your project structure
dotnet ef migrations remove

# 3. Completely delete the database file to ensure no ghost rows remain hidden in LocalDB
dotnet ef database drop --force

# 4. Generate a clean structural migration blueprint merging the new table schema
dotnet ef migrations add AddEmployeesAndRestructure

# 5. Re-apply everything fresh directly into LocalDB
dotnet ef database update


=========================================================================


Info to log for Employees:
Department
Name
Login/Logout


Password:
Admin123



To delete localDb:
dotnet ef database update



=========================================================================
Test the CSV Output:

.Run your web application via dotnet run
.Navigate to your History portal view
.Select any date and hit Generate Report
.Click the Download report (.CSV) button. Your browser will instantly invoke a file transfer utility saving your daily logs structured for direct ingestion into desktop software utilities like Microsoft Excel or Google Sheets.
=========================================================================
Password for the admin page:

To implement an editable password for your admin page without the full overhead of ASP.NET Core Identity, you can store a password hash in your LocalDB database, create a simple settings page to edit it, and use Cookie Authentication to protect the view.
=========================================================================
Pagination (Index):

.Skip((currentPage - 1) * pageSize): If you are visiting page 2, it skips the first 10 records (2-1) * 10.
.Take(10): It grabs exactly the next 10 items out of the database pile
.Math.Ceiling: If there are 11 check-ins today, it calculates 11 / 10 = 1.1, which rounds up to create exactly 2 pages.

=========================================================================
Pagination (Admin History):
How it runsWhen you filter for an older date (e.g., 2026-09-25) that contains 25 historical rows:The initial query pulls items 1 through 10.The page numbers render dynamic addresses targeting ?filterDate=2026-09-25&page=2.Clicking Next shifts the records smoothly to items 11 through 20 without losing your place in time.
=========================================================================
=========================================================================
Real-time clock:

The clock in this application is driven completely on the client side (browser window) using Vanilla JavaScript, rather than calling your backend server or LocalDB database.It works by instantly grabbing the exact date and time from the computer or device displaying the screen [INDEX] and using a continuous loop to update itself every single second.Here is the exact code block that sets and updates your clock inside Views/Attendance/Index.cshtml:javascriptfunction updateClock() {


💡 Crucial Operational Considerations for Your Kiosk:
Dependent on Kiosk System Time: Because the script calls new Date(), the clock displays the time of the device running the web browser. If the kiosk tablet or computer clock is incorrect or out of sync, the displayed time will be wrong, but the backend server will still write the database records using its own internal server clock [INDEX].
Zero Server Overhead: Since the timing engine executes entirely inside the worker's browser thread, it uses zero network bandwidth and zero database processing power [INDEX], keeping your MVC terminal extremely fast.

=========================================================================
=========================================================================
When entering decimal values:
On this application, I saw error message such as 'The two nearest value..''' but I can't find that

The error message you are seeing—or a variation of it—is actually not coming from your application's C# code or SQL database table logic.Instead, this is a standard, built-in HTML5 browser validation message that triggers when someone types a value into a numeric input field that doesn't align with the field's configuration constraints.

=========================================================================
=========================================================================
Messages like 'Successfully logged-in', etc.:
How TempData actually works under the hood

Short-Term Storage: TempData uses the server's session state to hold data for exactly one HTTP request.

The Hand-off: When the controller redirects back to the Index page (return RedirectToAction(nameof(Index));), it hands this message over to the next page load.

The Display & Destruction: When Index.cshtml reads @if (TempData["Message"] != null), it prints the alert box on the employee terminal. As soon as the page finishes rendering, .NET automatically erases the message from memory so it doesn't show up again if the user refreshes the page.
=========================================================================
=========================================================================
=========================================================================
=========================================================================
LocalDB Tables Export Guide

Option 1: Export Table Data or Schema as a Script
1. Open Visual Studio and go to View > SQL Server Object Explorer.
2. Expand SQL Server and select your local instance (usually (localdb)\MSSQLLocalDB).
3. Expand Databases, then expand your specific database.
4. Right-click the Tables folder (or a specific table) and select View Code to generate the CREATE TABLE script.
5. For a full export: Open SSMS, connect to (localdb)\MSSQLLocalDB, right-click the database, choose Tasks > Generate Scripts.

Option 2: Extract the Physical Database Files (.mdf / .ldf)
1. In Visual Studio, right-click the database in SQL Server Object Explorer and select Properties.
2. Locate the Data File path (.mdf file).
3. Right-click the database and select Detach.
4. Copy the .mdf and .ldf files from that folder path.

Option 3: Generate Tables from Code (EF Core Migrations)
1. Open Tools > NuGet Package Manager > Package Manager Console.
2. Run 'Update-Database' to push your code tables to LocalDB.
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
=========================================================================
Pendings:
1.Validations on Names (Should not be an integer)
2.How to display maximum of 10 lines only on the Index, then handle others on next pages(still at 10 lines only)
3.Add a filter to display dates of attendance on Index.cshtml


Home Page:
1.Url
2.Liitan ang Magandang Araw!


fs-2 (Approx. 32px) — Recommended. Moderately sized, professional, and fits nicely on small kiosk screens.fs-3 (Approx. 28px) — Smaller, matches standard section titles.fs-4 (Approx. 24px) — Compact, great if you want it to look like a subtitle.
2.Administration History Portal		> Admin
2.



1.
Sorting of employees, Check-in time, descending


 404 "Page Not Found" View

=========================================================================

Error page is at:
https://localhost:44328/Error

=========================================================================
Employee List:

/1Zarah Moran
/2Jose Rizal
/3Andres Bonifacio
/4Apolinario Mabini
/5Emilio Aguinaldo
/6Juan Luna
/7Peter Parker (Spider-Man)
/8Bruce Banner (The Hulk)
/9Matt Murdock (Daredevil)
/10Reed Richards (Mister Fantastic)
/11Sue Storm (Invisible Woman)
/12Scott Summers (Cyclops)
/13Stephen Strange (Doctor Strange)
/14Kamala Khan (Ms. Marvel)
/15Jessica Jones (Jessica Jones)
/16Bucky Barnes (Winter Soldier)
/17Zac Moran
/18Red Moran
/19Car Moran
/20Summer Moran
/21Caramel Moran
/22Garlicbird Moran


=========================================================================
=========================================================================
Publishing:

Netlify:
https://rococo-pika-0eee5c.netlify.app/

Vercel:
https://vercel.com/z8-47d3/attendance-monitoring-system

monsterasp.net:
zmattendancemonitoringsystem.runasp.net
Server=db70622.databaseasp.net; Database=db70622; User Id=db70622; Password=9Ac+_6Jga!3K; Encrypt=False; MultipleActiveResultSets=True;

=========================================================================
=========================================================================




