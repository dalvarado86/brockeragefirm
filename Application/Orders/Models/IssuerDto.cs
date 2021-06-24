using System.Text.Json.Serialization;

namespace Application.Orders.Models
{
    /// <summary>
    /// IssuerDto
    /// </summary>
    public class IssuerDto
    {
        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the total shares.
        /// </summary>
        [JsonPropertyName("total_shares")]
        public int TotalShares { get; set; }

        /// <summary>
        /// Gets or sets the share price.
        /// </summary>
        [JsonPropertyName("share_price")]
        public decimal SharePrice { get; set; }
    }
}
