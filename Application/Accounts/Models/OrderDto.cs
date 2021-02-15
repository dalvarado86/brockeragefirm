using System.Text.Json.Serialization;

namespace Application.Accounts.Models
{
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
}
