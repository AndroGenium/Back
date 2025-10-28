using Backend.Models;

namespace Backend.Requests
{
    public class AddReview
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } // 1-5 scale
    }
}
