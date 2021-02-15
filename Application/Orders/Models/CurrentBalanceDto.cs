using System.Collections.Generic;

namespace Application.Orders.Models
{
    public class CurrentBalanceDto
    {
        public decimal Cash { get; set; }
        public IList<IssuerDto> Issuers { get; set; } = new List<IssuerDto>();        
    }
}
