namespace Backend.Responses.ProductResponses
{
    public class AddProductResponse
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public bool? IsDonateable { get; set; }
        public int? LenderId { get; set; }
        public List<string>? ImageUrls { get; set; }
        public int Views { get; set; }
    }
}
