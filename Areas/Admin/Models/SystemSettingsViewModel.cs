using System.ComponentModel.DataAnnotations;

namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class SystemSettingsViewModel
    {
        public string DatabaseProvider { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string LastBackup { get; set; } = string.Empty;
        public bool MaintenanceMode { get; set; }
    }
}