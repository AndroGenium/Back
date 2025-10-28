using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Backend.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;




namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly Baza _db;
        public ReviewController(Baza db)
        {
            _db = db;
        }

        [HttpGet("get-reviews")]
        public ActionResult GetReviews()
        {
            return Ok(_db.Reviews);
        }

        [HttpPost("add-review")]
        public ActionResult AddReview(AddReview info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = _db.Products.FirstOrDefault(p => p.Id == info.ProductId);
            if (product == null)
                return NotFound("Product not found");

            var user = _db.Users.FirstOrDefault(u => u.Id == info.UserId);
            if (user == null)
                return NotFound("User not found");

            var review = new Review
            {
                Title = info.Title,
                Content = info.Content,
                Rating = info.Rating,
                Product = product,
                User = user
            };

            _db.Reviews.Add(review);
            _db.SaveChanges();

            return Ok(new
            {
                review.Title,
                review.Content,
                review.Rating,
                review.ProductId,
                review.UserId
            });
        }

        [HttpPost("add-multiple-reviews")]
        
        public ActionResult AddMultipleReviews(List<AddReview> reviewsInfo)
        {
            var ReviewsToAdd = new List<Review>();

            foreach (var info in reviewsInfo)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = _db.Products.FirstOrDefault(p => p.Id == info.ProductId);
                if (product == null)
                    return NotFound("Product not found");

                var user = _db.Users.FirstOrDefault(u => u.Id == info.UserId);
                if (user == null)
                    return NotFound("User not found");

                var review = new Review
                {
                    Title = info.Title,
                    Content = info.Content,
                    Rating = info.Rating,
                    Product = product,
                    User = user
                };

                ReviewsToAdd.Add(review);
            }

            _db.Reviews.AddRange(ReviewsToAdd);
            _db.SaveChanges();

            return Ok(ReviewsToAdd);
        }
    }



}
