using System.Collections.Generic;

namespace Application.Orders.Models
{
    /// <summary>
    /// CurrentBalanceDto.
    /// </summary>
    public class CurrentBalanceDto
    {
        /// <summary>
        /// Gets or sets the cash amount.
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Gets or sets a collection of issuers.
        /// </summary>
        public IList<IssuerDto> Issuers { get; set; } = new List<IssuerDto>();        
    }
}
