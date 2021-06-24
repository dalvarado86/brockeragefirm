using System.Text.Json.Serialization;

namespace Application.Accounts.Models
{
    /// <summary>
    /// StockDto.
    /// </summary>
    public class StockDto
    {
        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }
    }
}
