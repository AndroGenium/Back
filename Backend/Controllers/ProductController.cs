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


        [HttpPatch("edit-product-by-id/{ProductId}")]
        public ActionResult EditProduct(int ProductId, EditProduct info)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == ProductId);
            if (product == null)
                return NotFound(new ApiResponse(ErrorCodes.ProductNotFound, "Product not found", ModelState));

            // Apply updates if values are not null
            product.Name ??= info.Name;
            product.Category ??= info.Category;
            product.Description ??= info.Description;
            product.Views = info.Views ?? product.Views;
            product.IsDonateable = info.IsDonateable ?? product.IsDonateable;
            product.IsAvailable = info.IsAvailable ?? product.IsAvailable;
            product.MoneyRaised = info.MoneyRaised ?? product.MoneyRaised;
            product.LenderId = info.LenderId ?? product.LenderId;
            product.BorrowerId = info.BorrowerId ?? product.BorrowerId;
            product.ImageUrls ??= info.ImageUrls;

            if (info.LikedByUserIds != null)
            {
                product.LikedByUsers = _db.Users
                    .Where(u => info.LikedByUserIds.Contains(u.Id))
                    .ToList();
            }

            _db.SaveChanges();
            return Ok(product);
        }
    }
}
