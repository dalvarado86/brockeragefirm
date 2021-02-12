namespace Domain.Entities
{
    public class Issuer
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string Name { get; set; }
        public int TotalShares { get; set; }
        public decimal SharePrice { get; set; }
    }
}
