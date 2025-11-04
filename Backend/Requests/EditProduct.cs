using Backend.Models;

namespace Backend.Requests
{
    public class EditProduct
    {

        public string? Name { get; set; }
        public string? Category { get; set; }
        public int? Views { get; set; }
        public string? Description { get; set; }
        public bool? IsDonateable { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? MoneyRaised { get; set; }
        public int? LenderId { get; set; }
        public int? BorrowerId { get; set; }
        public List<int>? LikedByUserIds { get; set; }
        public List<string>? ImageUrls { get; set; }

    }
}
