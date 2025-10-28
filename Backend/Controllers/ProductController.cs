using Backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Requests;
using Backend.Models;
using Backend.Responses;
using Backend.Responses.ProductResponses;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Baza _db;
        public ProductController(Baza db)
        {
            _db = db;
        }

        [HttpGet("get-products")]
        public ActionResult GetProducts()
        {
            return Ok(_db.Products);
        }

        [HttpPost("add-product")]
        public ActionResult AddProduct(AddProduct info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _db.Users.FirstOrDefault(u => u.Id == info.LenderId);
            if (user == null)
                return Conflict(new ApiResponse(ErrorCodes.UserNotFound,"User not found", ModelState));


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
            
            if(info.ImageUrls != null)
                producttoadd.ImageUrls = info.ImageUrls;


            _db.Products.Add(producttoadd);
            _db.SaveChanges();

            var Response = new AddProductResponse
            {
                Id = producttoadd.Id,
                Name = producttoadd.Name,
                Category = producttoadd.Category,
                Description = producttoadd.Description,
                IsDonateable = producttoadd.IsDonateable,
                LenderId = producttoadd.LenderId,
                ImageUrls = producttoadd.ImageUrls,
                Views = producttoadd.Views
            };

            return Ok(Response);
        }

        [HttpPost("add-multiple-products")]

        public ActionResult AddMultipleProducts(List<AddProduct> ProductsInfo)
        {
            var ProductsToAdd = new List<Product>();
            var Responses = new List<AddProductResponse>();

            foreach(var info in ProductsInfo)
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
    }
}
