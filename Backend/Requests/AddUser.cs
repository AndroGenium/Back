using Backend.Enums;
using Backend.Models;

namespace Backend.Requests
{
    public class AddUser
    {


        //Auth
        public string Email { get; set; } // <---
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string password { get; set; }
        public string BirthDate { get; set; } // <---
        public List<Product>? LikedProducts {  get; set; }


    }
}
