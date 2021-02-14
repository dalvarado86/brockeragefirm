namespace Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string IssuerName { get; set; }
        public int Quantity { get; set; }
    }
}
