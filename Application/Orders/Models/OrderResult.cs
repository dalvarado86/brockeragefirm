using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Orders.Models
{
    public class OrderResult
    {

        [JsonPropertyName("current_balance")]
        public CurrentBalanceDto CurrentBalance { get; set; }

        [JsonPropertyName("business_errors")]
        public IList<string> BusinessErrors { get; set; } = new List<string>();

    }
}
