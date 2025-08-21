using System.ComponentModel.DataAnnotations;
using SenegaleseAssociation.Data;

namespace SenegaleseAssociation.Models
{
    public class Testimonial : ITimestamped
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string AuthorTitle { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string AuthorImageUrl { get; set; } = string.Empty;
        
        public int Rating { get; set; } = 5;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsFeatured { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}