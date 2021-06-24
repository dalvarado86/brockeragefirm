using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Validators;
using Application.Orders.Models;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orders
{
    /// <summary>
    /// CreateOrderCommand
    /// </summary>
    public class CreateOrderCommand : IRequest<OrderResult>
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        [JsonIgnore]
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public string Operation { get; set; }   
        
        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the total shares.
        /// </summary>
        [JsonPropertyName("total_shares")]
        public int TotalShares { get; set; }

        /// <summary>
        /// Gets or sets the share price.
        /// </summary>
        [JsonPropertyName("share_price")]
        public decimal SharePrice { get; set; }
    }

    /// <summary>
    /// CommandValidator
    /// </summary>
    public class CommandValidator : AbstractValidator<CreateOrderCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        public CommandValidator()
        {
            RuleFor(x => x.Operation)
                .NotEmpty();

            RuleFor(x => x.IssuerName)
                .NotEmpty();

            RuleFor(x => x.TotalShares)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.SharePrice)
               .NotEmpty()
               .GreaterThan(0);
        }
    }

    /// <summary>
    /// Handler.
    /// </summary>
    public class Handler : IRequestHandler<CreateOrderCommand, OrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<MarketSettings> _marketSettings;
        private readonly ILogger<Handler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="marketSettings">The market settings.</param>
        /// <param name="logger">The logger.</param>
        public Handler(
            IApplicationDbContext context, 
            IMapper mapper, 
            IOptions<MarketSettings> marketSettings,
            ILogger<Handler> logger)
        {
            _mapper = mapper;
            _context = context;
            _marketSettings = marketSettings;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts
                .Include(x => x.Orders)
                .Include(x => x.Stocks)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.AccountId);

            if (account == null)
            {
                _logger.LogInformation($"The account '{request.AccountId}' not found");
                throw new RestException(HttpStatusCode.NotFound, new { Account = "Not found" });
            }

            _logger.LogInformation($"Account retrieved: {account}");

            var order = new Order
            {
                AccountId = request.AccountId,
                TimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Operation = request.Operation,
                IssuerName = request.IssuerName.Trim(),
                TotalShares = request.TotalShares,
                SharePrice = request.SharePrice
            };

            _logger.LogDebug("Total: SharePrice * TotalShares");
            var grandTotal = request.SharePrice * request.TotalShares;
            _logger.LogDebug($"Total: {request.SharePrice} * {request.TotalShares} = {grandTotal}");

            _logger.LogInformation("Validate bussiness rules.");
            var businessErrors = Validations(account, order);
                          
            if (businessErrors.Count == 0)
            {
                _logger.LogInformation($"Add order: {order}");
                _context.Orders.Add(order);

                // Getting the specific stock from the account
                var stock = account.Stocks
                    .SingleOrDefault(x => x.IssuerName == order.IssuerName);

                if (stock != null)
                {
                    _logger.LogInformation($"Update stock");
                    stock.Quantity = string.Equals(request.Operation, "BUY") 
                        ? stock.Quantity += request.TotalShares // Adding to the stock if is buy
                        : stock.Quantity -= request.TotalShares; // Removing to the stock if is sell
                }
                else
                {
                    stock = new Stock
                    {
                        AccountId = request.AccountId,
                        IssuerName = request.IssuerName,
                        Quantity = request.TotalShares
                    };

                    _logger.LogInformation($"Add new stock");
                    _context.Stocks.Add(stock); // Creating new element in the account's stock
                }

                _logger.LogInformation($"Update account balance");
                account.Cash = string.Equals(request.Operation, "SELL") 
                    ? account.Cash += grandTotal 
                    : account.Cash -= grandTotal;
               
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!success)
                {
                    _logger.LogError("There are a problem creating the order", nameof(request));
                    throw new Common.Exceptions.ApplicationException("There are a problem creating the order");
                }
            }

            var result = new OrderResult
            {
                CurrentBalance = new CurrentBalanceDto
                {
                    Cash = account.Cash,
                    Issuers = _mapper.Map<List<Order>, List<IssuerDto>>((List<Order>)account.Orders)
                },
                BusinessErrors = businessErrors
            };

            _logger.LogInformation($"Result: {result}");

            return result;
        }

        private List<string> Validations(Account account, Order order)
        {
            var errors = new List<string>();

            switch (order.Operation)
            {
                case "BUY":
                    if (!BusinessRulesValidator.SufficentFunds(account, order))
                        errors.Add("INSUFFICIENT_BALANCE");
                    break;
                case "SELL":
                    if (!BusinessRulesValidator.SufficentStocks(account, order))
                        errors.Add("INSUFFICIENT_STOCKS");
                    break;
                default:
                    errors.Add("INVALID_OPERATION");
                    break;
            }

            if (BusinessRulesValidator.Duplicated(account, order))
            {
                errors.Add("DUPLICATED_OPERATION");
            }

            if (!BusinessRulesValidator.MarketIsOpen(_marketSettings.Value.TimeOpen, _marketSettings.Value.TimeClose))
            {
                errors.Add("CLOSED_MARKET");
            }

            return errors;
        }
    }
}
