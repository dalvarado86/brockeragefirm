using System.Collections.Generic;

namespace Application.Accounts.Models
{
    public class AccountResult
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        public IList<string> Issuers { get; private set; } = new List<string>();
    }
}
