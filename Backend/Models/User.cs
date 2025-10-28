namespace Backend.Models;
using Backend.Enums;
using Microsoft.OpenApi.Any;

public class User
{
    //Auth
    public int Id { get; set; }
    public string Email { get; set; } // <---

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? PhoneNumber { get; set; }

    private string _password;
    public string Password
    {
        get => "User Password";
        set
        {
            _password = HashPass(value);
        }
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
    public List<Review>? Reviews { get; set; }

    //


    public bool IsVerified { get; set; } = false;       // Email Verified- 1 
    public bool IsBanned { get; set; } = false;         
    public DateTime? LastLogin { get; set; }            
    public DateTime? DateCreated { get; set; } = DateTime.Now;


    public bool VerifyPass(string input)
    {
        return BCrypt.Net.BCrypt.Verify(input, _password);
    }

    public string HashPass(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }
}
