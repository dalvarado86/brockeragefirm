using Domain.Entities;
using System;
using System.Linq;

namespace Application.Common.Validators
{
    public static class BusinessRulesValidator
    {
        public static bool SufficentFunds(Account account, Order order)
        {
            return (account.Cash >= (order.SharePrice * order.TotalShares));
        }

        public static bool SufficentStocks(Account account, Order order)
        {
            return account.Stocks
                .Where(x => x.IssuerName == order.IssuerName)
                .SingleOrDefault()?.Quantity >= order.TotalShares;
        }

        public static bool Duplicated(Account account, Order order)
        {
            return account.Orders
                .Any(x => x.IssuerName == order.IssuerName &&
                            x.Operation == order.Operation &&
                                x.SharePrice == order.SharePrice &&
                                     GetDiferenceInMinutes(x.TimeStamp, order.TimeStamp) <= 5);
        }

        public static bool MarketIsOpen(TimeSpan timeStart, TimeSpan timeEnd)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now > timeStart && now < timeEnd;
        }

        private static int GetDiferenceInMinutes(long timespanStart, long timespanEnd)
        {
            var dateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeStart = dateTimeStart.AddSeconds(timespanStart).ToLocalTime();

            var dateTimeEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeEnd = dateTimeEnd.AddSeconds(timespanEnd).ToLocalTime();

            return (dateTimeEnd - dateTimeStart).Minutes;
        }    
    }
}