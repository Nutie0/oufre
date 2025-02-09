using System;
using System.ComponentModel.DataAnnotations;
namespace RegisterAPI.Models;

public class User
{
    public int UserId { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; }
    
    [Required, StringLength(50)]
    public string Username { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    public bool IsVerified { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? TokenExpires { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
