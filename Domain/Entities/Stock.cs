namespace Domain.Entities
{
    /// <summary>
    /// Stock.
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Gets or sets the stock identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }
    }
}
