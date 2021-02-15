using System.Text.Json.Serialization;

namespace Application.Accounts.Models
{
    public class StockDto
    {
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }
        public int Quantity { get; set; }
    }
}
