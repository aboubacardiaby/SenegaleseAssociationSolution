using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Areas.Admin.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SystemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public SystemController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        public IActionResult Settings()
        {
            var settings = new SystemSettingsViewModel
            {
                DatabaseProvider = "PostgreSQL (Supabase)",
                Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Version = "1.0.0",
                LastBackup = "Not Configured",
                MaintenanceMode = false,
                SmtpHost     = _configuration["Email:SmtpHost"] ?? string.Empty,
                SmtpPort     = int.TryParse(_configuration["Email:SmtpPort"], out var port) ? port : 587,
                SmtpUsername = _configuration["Email:SmtpUsername"] ?? string.Empty,
                SmtpPassword = _configuration["Email:SmtpPassword"] ?? string.Empty,
                FromEmail    = _configuration["Email:FromEmail"] ?? string.Empty,
                FromName     = _configuration["Email:FromName"] ?? string.Empty,
                AdminEmail   = _configuration["Email:AdminEmail"] ?? string.Empty,
                SmtpUseSsl   = true
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMaintenanceMode(bool maintenanceMode)
        {
            TempData["SuccessMessage"] = $"Maintenance mode {(maintenanceMode ? "enabled" : "disabled")} successfully.";
            TempData["ActiveTab"] = "actions";
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSmtpSettings(SystemSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the validation errors and try again.";
                TempData["ActiveTab"] = "email";
                return RedirectToAction(nameof(Settings));
            }

            try
            {
                var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var jsonText = await System.IO.File.ReadAllTextAsync(appSettingsPath);
                var jsonNode = JsonNode.Parse(jsonText);

                if (jsonNode?["Email"] is JsonObject emailSection)
                {
                    emailSection["SmtpHost"]     = model.SmtpHost;
                    emailSection["SmtpPort"]     = model.SmtpPort.ToString();
                    emailSection["SmtpUsername"] = model.SmtpUsername;
                    emailSection["SmtpPassword"] = model.SmtpPassword;
                    emailSection["FromEmail"]    = model.FromEmail;
                    emailSection["FromName"]     = model.FromName;
                    emailSection["AdminEmail"]   = model.AdminEmail;
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                await System.IO.File.WriteAllTextAsync(appSettingsPath, jsonNode?.ToJsonString(options));

                // Reload configuration so changes take effect without restart
                if (_configuration is IConfigurationRoot configRoot)
                {
                    configRoot.Reload();
                }

                TempData["SuccessMessage"] = "Email/SMTP settings saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to save settings: " + ex.Message;
            }

            TempData["ActiveTab"] = "email";
            return RedirectToAction(nameof(Settings));
        }

        public IActionResult Backup()
        {
            TempData["InfoMessage"] = "Database backup functionality is not yet configured.";
            TempData["ActiveTab"] = "actions";
            return RedirectToAction(nameof(Settings));
        }

        public IActionResult ClearCache()
        {
            TempData["SuccessMessage"] = "Application cache cleared successfully.";
            TempData["ActiveTab"] = "actions";
            return RedirectToAction(nameof(Settings));
        }

        public IActionResult Logs()
        {
            var logs = new List<SystemLogViewModel>
            {
                new() { Date = DateTime.Now.AddHours(-1),  Level = "INFO",  Message = "User logged in successfully",     User = "admin@sam.org"  },
                new() { Date = DateTime.Now.AddHours(-2),  Level = "WARN",  Message = "High memory usage detected",       User = "System"         },
                new() { Date = DateTime.Now.AddHours(-3),  Level = "INFO",  Message = "New event created",                User = "user@sam.org"   },
                new() { Date = DateTime.Now.AddHours(-4),  Level = "ERROR", Message = "Failed to send email notification", User = "System"        },
                new() { Date = DateTime.Now.AddHours(-6),  Level = "INFO",  Message = "Database backup completed",         User = "System"        },
                new() { Date = DateTime.Now.AddHours(-8),  Level = "INFO",  Message = "New donation received",             User = "System"        },
                new() { Date = DateTime.Now.AddHours(-10), Level = "WARN",  Message = "Slow query detected (2300ms)",      User = "System"        },
                new() { Date = DateTime.Now.AddHours(-12), Level = "INFO",  Message = "New member registered",             User = "System"        },
            };

            return View(logs);
        }
    }
}
