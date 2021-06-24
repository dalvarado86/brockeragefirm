using System.Text.Json.Serialization;

namespace Application.Accounts.Models
{
    /// <summary>
    /// OrderDto.
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        public string Operation { get; set; }

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
