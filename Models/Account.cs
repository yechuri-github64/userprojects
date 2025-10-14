using System;
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
    public string? Address { get; set; }  // nullable

    [MaxLength(50)]
    [Column("status")]
    public string? Status { get; set; }   // nullable

    [Column("balance", TypeName = "decimal(19,4)")]
    public decimal? Balance { get; set; }  // nullable

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; } // nullable

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } // nullable
}
