using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Orders.Models
{
    /// <summary>
    /// OrderResult.
    /// </summary>
    public class OrderResult
    {
        /// <summary>
        /// Gets or sets the current balance.
        /// </summary>
        [JsonPropertyName("current_balance")]
        public CurrentBalanceDto CurrentBalance { get; set; }

        /// <summary>
        /// Gets or sets the business errors.
        /// </summary>
        [JsonPropertyName("business_errors")]
        public IList<string> BusinessErrors { get; set; } = new List<string>();

    }
}
