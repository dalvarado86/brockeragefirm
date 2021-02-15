using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Accounts
{
    public class AccountDetailsDto
    {
        [JsonPropertyName("account_holder")]
        public string AccountHolderUser { get; set; }
        public decimal Cash { get; set; }
        public IList<StockDto> Stock { get; set; } = new List<StockDto>();

        [JsonPropertyName("order_history")]
        public IList<OrderDto> Orders { get; set; } = new List<OrderDto>();      
    }

    public class OrderDto
    {
        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }
        public string Operation { get; set; }

        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        [JsonPropertyName("total_shares")]
        public int TotalShares { get; set; }

        [JsonPropertyName("share_price")]
        public decimal SharePrice { get; set; }
    }

    public class StockDto
    {
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }
        public int Quantity { get; set; }
    }
}
