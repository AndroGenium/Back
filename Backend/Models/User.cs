namespace Backend.Models;
using Backend.Enums;
using Microsoft.OpenApi.Any;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    //Auth

    [Key]
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } // <---

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }

    [Required]
    public string PasswordHash { get; private set; }

    [NotMapped]
    public string Password
    {
        set => PasswordHash = BCrypt.Net.BCrypt.HashPassword(value);
    }

    //Profile
    public DateTime DateOfBirth { get; set; }
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public string? Bio { get; set; } 
    public string? ProfilePictureUrl { get; set; }
    public string? Location { get; set; }

    //Permisions
    public UserRole Permissions { get; set; } = UserRole.User;

    //Commerce Specific Properties

    public decimal? Balance { get; set; }  = 0.0m;
    public float? AverageRating { get; set; } = 0.0f;    

    //
    public List<Product>? ListedProducts { get; set; }
    public List<Product>? BorrowedProducts { get; set; }
    public List<Product>? LikedProducts { get; set; }

    //


    public bool IsVerified { get; set; } = false;       // Email Verified- 1 
    public bool IsBanned { get; set; } = false;         
    public DateTime? LastLogin { get; set; }            
    public DateTime? DateCreated { get; set; } = DateTime.Now;


    public bool VerifyPass(string input)
    {
        return BCrypt.Net.BCrypt.Verify(input, PasswordHash);
    }

}
