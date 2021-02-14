using System.Collections.Generic;

namespace Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        public IList<Stock> Stocks { get; private set; } = new List<Stock>();
        public IList<Order> Orders { get; private set; } = new List<Order>();
    }
}
