using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SenegaleseAssociation.Data;

namespace SenegaleseAssociation.Models
{
    public class Donation : ITimestamped
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 1000000, ErrorMessage = "Donation amount must be between $1 and $1,000,000")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Frequency { get; set; } = "OneTime"; // OneTime, Monthly, Annual

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // PayPal, Zelle, Venmo, ACH

        [StringLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string DonorEmail { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string DonorPhone { get; set; } = string.Empty;

        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsAnonymous { get; set; } = false;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Cancelled

        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime? ProcessedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for tracking who processed the donation (if applicable)
        public string? ProcessedById { get; set; }

        public ApplicationUser? ProcessedBy { get; set; }
    }
}
