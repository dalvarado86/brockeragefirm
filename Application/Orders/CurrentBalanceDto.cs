using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Orders
{
    public class OrderResult
    {

        [JsonPropertyName("current_balance")]
        public CurrentBalance CurrentBalance { get; set; }

        [JsonPropertyName("business_errors")]
        public IList<string> BusinessErrors { get; set; } = new List<string>();

    }

    public class CurrentBalance
    {
        public decimal Cash { get; set; }
        public IList<IssuerDto> Issuers { get; set; } = new List<IssuerDto>();        
    }

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
