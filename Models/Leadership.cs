using System.ComponentModel.DataAnnotations;
using SenegaleseAssociation.Data;

namespace SenegaleseAssociation.Models
{
    public class Leadership : ITimestamped
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string WelcomeMessage { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public DateTime ServiceStartDate { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsPresident { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string FullName => $"{FirstName} {LastName}";
    }
}