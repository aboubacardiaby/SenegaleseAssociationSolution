using System.ComponentModel.DataAnnotations;
using SenegaleseAssociation.Data;

namespace SenegaleseAssociation.Models
{
    public class ContactMessage : ITimestamped
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Subject")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Message")]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}