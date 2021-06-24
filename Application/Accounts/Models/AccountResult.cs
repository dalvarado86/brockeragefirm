using System.Collections.Generic;

namespace Application.Accounts.Models
{
    /// <summary>
    /// AccountResult.
    /// </summary>
    public class AccountResult
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the cash ammount.
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Gets or sets a collection of issuers.
        /// </summary>
        public IList<string> Issuers { get; private set; } = new List<string>();
    }
}
