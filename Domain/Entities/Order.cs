namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }      
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public long TimeStamp { get; set; }
        public string Operation { get; set; }
        public string IssuerName { get; set; }
        public int TotalShares { get; set; }
        public decimal SharePrice { get; set; }
    }
}
