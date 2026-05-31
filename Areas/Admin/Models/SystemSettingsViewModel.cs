using System.ComponentModel.DataAnnotations;

namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class SystemSettingsViewModel
    {
        // System Information
        public string DatabaseProvider { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string LastBackup { get; set; } = string.Empty;
        public bool MaintenanceMode { get; set; }

        // SMTP / Email Settings
        [Display(Name = "SMTP Host")]
        public string SmtpHost { get; set; } = string.Empty;

        [Display(Name = "SMTP Port")]
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "SMTP Username")]
        public string SmtpUsername { get; set; } = string.Empty;

        [Display(Name = "SMTP Password")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; } = string.Empty;

        [Display(Name = "From Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string FromEmail { get; set; } = string.Empty;

        [Display(Name = "From Name")]
        public string FromName { get; set; } = string.Empty;

        [Display(Name = "Admin Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string AdminEmail { get; set; } = string.Empty;

        [Display(Name = "Enable SSL/TLS")]
        public bool SmtpUseSsl { get; set; } = true;
    }
}
