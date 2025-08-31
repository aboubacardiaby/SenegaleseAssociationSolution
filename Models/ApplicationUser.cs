using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SenegaleseAssociation.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        public string FullName => $"{FirstName} {LastName}";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        public DateTime? LastActivityAt { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}