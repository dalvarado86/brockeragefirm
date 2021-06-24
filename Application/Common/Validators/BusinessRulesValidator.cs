using Domain.Entities;
using System;
using System.Linq;

namespace Application.Common.Validators
{
    /// <summary>
    /// BusinessRulesValidator.
    /// </summary>
    public static class BusinessRulesValidator
    {
        /// <summary>
        /// Validates if there are sufficient funds in a specific account to compute a specific order.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="order">The order.</param>
        /// <returns><c>true</c> if there are sufficient funds, otherwise <c>false</c></returns>
        public static bool SufficentFunds(Account account, Order order)
        {
            return (account.Cash >= (order.SharePrice * order.TotalShares));
        }

        /// <summary>
        /// Validates if there are, in the stock, sufficient issuers to sell.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="order">The order.</param>
        /// <returns><c>true</c> if there are sufficient issuers, otherwise <c>false</c></returns>
        public static bool SufficentStocks(Account account, Order order)
        {
            return account.Stocks
                .SingleOrDefault(x => x.IssuerName == order.IssuerName)?.Quantity >= order.TotalShares;
        }

        /// <summary>
        /// Validates if the order is duplicated.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="order">The order.</param>
        /// <returns><c>true</c> if the operation is duplicated, otherwise <c>false</c></returns>
        /// <remarks>
        /// An operation is duplicated if is the same issuer, the same operation and the same price within a period of 5 minutes.
        /// </remarks>
        public static bool Duplicated(Account account, Order order)
        {
            return account.Orders
                .Any(x => x.IssuerName == order.IssuerName &&
                            x.Operation == order.Operation &&
                                x.SharePrice == order.SharePrice &&
                                     GetDiferenceInMinutes(x.TimeStamp, order.TimeStamp) <= 5);
        }

        /// <summary>
        /// Validates if the market is open.
        /// </summary>
        /// <param name="timeStart">The time start.</param>
        /// <param name="timeEnd">The time end.</param>
        /// <returns><c>true</c> if the market is open, otherwise <c>false</c></returns>
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