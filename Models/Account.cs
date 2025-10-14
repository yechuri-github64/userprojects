using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace accounts_management.Models;

[Table("accounts")]
public class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("address")]
    public string Address { get; set; } = string.Empty;
}
