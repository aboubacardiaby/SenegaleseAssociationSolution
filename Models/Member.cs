using System.ComponentModel.DataAnnotations;

namespace SenegaleseAssociation.Models
{
    public class Member
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Profession { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string EmergencyContactName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string EmergencyContactPhone { get; set; } = string.Empty;
        
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        [StringLength(20)]
        public string MembershipStatus { get; set; } = "Pending"; // Pending, Active, Inactive, Suspended
        
        public DateTime? ApprovedDate { get; set; }
        
        public string? ApprovedById { get; set; }
        
        public ApplicationUser? ApprovedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string FullName => $"{FirstName} {LastName}";
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}