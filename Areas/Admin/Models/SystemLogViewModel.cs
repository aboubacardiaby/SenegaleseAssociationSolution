namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class SystemLogViewModel
    {
        public DateTime Date { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }
}