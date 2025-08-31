namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class MemberListViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string MembershipStatus { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public bool IsActive { get; set; }
    }
}