namespace Domain.Entities
{
    /// <summary>
    /// Order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the total shares.
        /// </summary>
        public int TotalShares { get; set; }

        /// <summary>
        /// Gets or sets the share price.
        /// </summary>
        public decimal SharePrice { get; set; }
    }
}
