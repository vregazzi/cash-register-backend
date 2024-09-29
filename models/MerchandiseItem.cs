namespace CashRegister.Models
{
    public class MerchandiseItem
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Price { get; set; }
        public required int Quantity { get; set; }
    }
}
