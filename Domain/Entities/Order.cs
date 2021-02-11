namespace Domain.Entities
{
    public class Order
    {
        public int TimeStamp { get; set; }
        public string Operation { get; set; }
        public Issuer Issuer { get; set; }
    }
}
