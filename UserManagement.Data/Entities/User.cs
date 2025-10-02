using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required]
    public string Forename { get; set; } = default!;
    [Required]
    public string Surname { get; set; } = default!;
    [Required]
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    [Required(ErrorMessage = "Please enter a valid date of birth")]
    public DateTime DateOfBirth { get; set; }
}
