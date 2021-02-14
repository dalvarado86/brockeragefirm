using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validators
{
    public static class BusinessValidator
    {
        private static readonly TimeSpan TimeStart = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan TimeEnd = new TimeSpan(15, 0, 0);

        public static List<string> Validate(Account account, Order order)
        {
            var businessErrors = new List<string>();
            
            if(string.Equals(order.Operation, "BUY"))
            {
                if (!SufficentFunds(account, order))
                    businessErrors.Add("INSUFFICENT_BALANCE");
            }

            if (string.Equals(order.Operation, "SELL"))
            {
                if (!SufficentStocks(account, order))
                    businessErrors.Add("INSUFFICENT_STOCKS");
            }

            var duplicated = account.Orders
                .Any(x => x.IssuerName == order.IssuerName && 
                            x.SharePrice == order.SharePrice &&
                                 GetMinutes(x.TimeStamp, order.TimeStamp) <= 5);
            if (duplicated)
                businessErrors.Add("DUPLICATED_OPERATION");

            if (!MarketIsOpen(TimeStart, TimeEnd))
                businessErrors.Add("CLOSED_MARKET");

            // TODO: Add Any other invalid operation

            return businessErrors;
        }

        // TODO: Set as configuration params
        private static bool MarketIsOpen(TimeSpan timeStart, TimeSpan timeEnd)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now < timeStart && now > timeEnd;
        }

        private static int GetMinutes(long dateStart, long dateEnd)
        {
            var dateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeStart = dateTimeStart.AddSeconds(dateStart).ToLocalTime();

            var dateTimeEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeEnd = dateTimeEnd.AddSeconds(dateEnd).ToLocalTime();

            return (dateTimeEnd - dateTimeStart).Minutes;
        }

        public static bool SufficentFunds(Account account, Order order)
        {
            if (account.Cash < (order.SharePrice * order.TotalShares))
                return false;

            return true;
        }

        public static bool SufficentStocks(Account account, Order order)
        {
            //TODO: Create a Stock entity and persistence
            var currentQtyIssuers = account.Orders
                .Where(x => x.IssuerName == order.IssuerName)
                .ToList().Count;

            // Check insufficent stock
            if (currentQtyIssuers < order.TotalShares)
                return false;

            return true;
        }
    }
}
