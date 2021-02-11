using System.Collections.Generic;

namespace Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        // TODO: will be Issuer treat as a value object?
        //public IList<Issuer> Issuers { get; private set; } = new List<Issuer>();
    }
}
