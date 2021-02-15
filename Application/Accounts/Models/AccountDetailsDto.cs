using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Accounts.Models
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
}
