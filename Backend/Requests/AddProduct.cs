using Backend.Models;

namespace Backend.Requests
{
    public class AddProduct
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public string? Description { get; set; }
        public bool IsDonateable { get; set; } = true;
        public int LenderId { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
