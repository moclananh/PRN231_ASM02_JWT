namespace Client.Models
{
    public class ProductVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
        public string? Color { get; set; }
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
