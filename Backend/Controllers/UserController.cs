using Backend.Data;
using Backend.Models;
using Backend.Requests;
using Backend.Responses;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Baza _db;
        private readonly ProcessDateInfo _dateformater;
        public UserController(Baza db, ProcessDateInfo dateformater)
        {
            _db = db;
            _dateformater = dateformater;
        }

        [HttpGet("get-all-users")]
        public ActionResult GetAllUsers()
        {
            return Ok(_db.Users);
        }

        [HttpGet("get-user-by-id/{id}")]
        public ActionResult GetUser(int id)
        {
            Baza db =new Baza();

            return Ok(_db.Users.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost("merge-likes/{userId}")]
        public IActionResult MergeLikes(int userId, [FromBody] List<int> productIds)
        {
            var user = _db.Users.Include(u => u.LikedProducts).FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var products = _db.Products.Where(p => productIds.Contains(p.Id)).ToList();

            foreach (var product in products)
            {
                if (!user.LikedProducts.Contains(product))
                    user.LikedProducts.Add(product);
            }

            _db.SaveChanges();
            return Ok(user.LikedProducts);
        }



    }
}
