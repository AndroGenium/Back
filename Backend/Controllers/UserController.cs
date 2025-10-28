using Backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Requests;
using Backend.Models;
using Backend.Responses;
using Backend.Services;

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

        [HttpGet("get-user-by-id")]
        public ActionResult GetUser(int id)
        {
            Baza db =new Baza();

            return Ok(_db.Users.FirstOrDefault(x => x.Id == id));
        }


        [HttpPost("add-user")]
        public ActionResult AddUser(AddUser RequestInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_db.Users.Any(u => u.Email == RequestInfo.Email))
            {
                return Conflict(new ApiResponse(ErrorCodes.EmailInUse,"Email Is In Use", ModelState));
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

            _db.Users.Add(UserToAdd);
            _db.SaveChanges();

            return Ok(UserToAdd);
            
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
                    return Conflict(new ApiResponse(ErrorCodes.EmailInUse,"Email Is In Use", ModelState));
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

    }
}
