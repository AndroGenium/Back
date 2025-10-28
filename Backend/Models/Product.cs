namespace Backend.Models
{
    public class Product
    {
        private static readonly Random _rand = new Random();


        public int Id { get; set; }
        public string Name { get; set; }    
        public string Category { get; set; }
        public int Views { get; set; } = 0;

        public string? Description { get; set; }
        public bool IsDonateable { get; set; } = true;
        public bool IsAvailable { get; set; } = true;

        public decimal MoneyRaised { get; set; } = _rand.Next(0, 250);

        public int LenderId { get; set; }
        public User? Lender { get; set; }
        
        public int? BorrowerId { get; set; }

        public User? Borrower { get; set; }

        public List<User>? LikedByUsers { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<Review>? Reviews { get; set; }















    }
}
