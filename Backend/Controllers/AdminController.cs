using Backend.Data;
using Backend.Models;
using Backend.Requests;
using Backend.Responses;
using Backend.Responses.ProductResponses;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly Baza _db;
        private readonly ProcessDateInfo _dateformater;
        public AdminController(Baza db, ProcessDateInfo dateformater)
        {
            _db = db;
            _dateformater = dateformater;
        }

        [HttpPost("add-multiple-users")]

        public ActionResult AddMultipleUsers(List<AddUser> UsersInfo)
        {
            List<User> UsersToAdd = new List<User>();
            foreach (var RequestInfo in UsersInfo)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (_db.Users.Any(u => u.Email == RequestInfo.Email))
                {
                    return Conflict(new ApiResponse(ErrorCodes.EmailInUse, "Email Is In Use", ModelState));
                }
                var UserToAdd = new User
                {
                    FirstName = RequestInfo.FirstName,
                    LastName = RequestInfo.LastName,
                    Password = RequestInfo.password,
                    Email = RequestInfo.Email,
                    DateOfBirth = _dateformater.ConvertFormatToTime(RequestInfo.BirthDate),
                    DateCreated = DateTime.Now,
                };
                UsersToAdd.Add(UserToAdd);
            }
            _db.Users.AddRange(UsersToAdd);
            _db.SaveChanges();
            return Ok(UsersToAdd);
        }

        [HttpPost("add-multiple-products")]

        public ActionResult AddMultipleProducts(List<AddProduct> ProductsInfo)
        {
            var ProductsToAdd = new List<Product>();
            var Responses = new List<AddProductResponse>();

            foreach (var info in ProductsInfo)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _db.Users.FirstOrDefault(u => u.Id == info.LenderId);
                if (user == null)
                    return Conflict(new ApiResponse(ErrorCodes.UserNotFound, "User not found", ModelState));

                var producttoadd = new Product
                {
                    Name = info.Name,
                    Category = info.Category,
                };
                if (info.Description != null)
                    producttoadd.Description = info.Description;

                producttoadd.IsDonateable = info.IsDonateable;
                producttoadd.LenderId = info.LenderId;

                producttoadd.Lender = _db.Users.FirstOrDefault(user => user.Id == info.LenderId);

                if (info.ImageUrls != null)
                    producttoadd.ImageUrls = info.ImageUrls;

                ProductsToAdd.Add(producttoadd);
            }
            _db.Products.AddRange(ProductsToAdd);
            _db.SaveChanges();

            Responses = ProductsToAdd.Select(p => new AddProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                IsDonateable = p.IsDonateable,
                LenderId = p.LenderId,
                ImageUrls = p.ImageUrls,
                Views = p.Views

            }).ToList();

            return Ok(Responses);
        }


        [HttpDelete("nuke-database")]

        public ActionResult NukeDatabase()
        {
            _db.Products.RemoveRange(_db.Products);
            _db.Users.RemoveRange(_db.Users);
            _db.SaveChanges();
            return Ok(new { Message = "Database nuked successfully." });
        }


    }
}
