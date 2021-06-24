using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Accounts.Models
{
    /// <summary>
    /// AccountDetailsDto.
    /// </summary>
    public class AccountDetailsDto
    {
        /// <summary>
        /// Gets or sets the account holder.
        /// </summary>
        [JsonPropertyName("account_holder")]
        public string AccountHolderUser { get; set; }

        /// <summary>
        /// Gets or sets the cash amount.
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Gets or sets the stock of the account.
        /// </summary>
        public IList<StockDto> Stock { get; set; } = new List<StockDto>();

        /// <summary>
        /// Gets or sets the order history.
        /// </summary>
        [JsonPropertyName("order_history")]
        public IList<OrderDto> Orders { get; set; } = new List<OrderDto>();      
    }
}
