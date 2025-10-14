using System.ComponentModel.DataAnnotations;

namespace accounts_management.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}
