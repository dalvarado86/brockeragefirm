using System.Text.Json.Serialization;

namespace Application.Orders.Models
{
    public class IssuerDto
    {
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        [JsonPropertyName("total_shares")]
        public int TotalShares { get; set; }

        [JsonPropertyName("share_price")]
        public decimal SharePrice { get; set; }
    }
}
