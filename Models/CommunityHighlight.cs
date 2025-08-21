using System.ComponentModel.DataAnnotations;
using SenegaleseAssociation.Data;

namespace SenegaleseAssociation.Models
{
    public class CommunityHighlight : ITimestamped
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string IconClass { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string BackgroundClass { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Stat1Number { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Stat1Label { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Stat2Number { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Stat2Label { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Stat3Number { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Stat3Label { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}