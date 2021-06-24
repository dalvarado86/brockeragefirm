using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// Account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the cash in the account.
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Gets the account's stock.
        /// </summary>
        public IList<Stock> Stocks { get; private set; } = new List<Stock>();

        /// <summary>
        /// Gets the account's orders.
        /// </summary>
        public IList<Order> Orders { get; private set; } = new List<Order>();

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user of the account.
        /// </summary>
        public ApplicationUser User { get; set; }
    }
}
