﻿using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orders
{
    public class CreateOrderCommand : IRequest<OrderResult>
    {
        [JsonIgnore]
        public int AccountId { get; set; }
        public long Timestamp { get; set; }
        public string Operation { get; set; }   
        
        [JsonPropertyName("issuer_name")]
        public string IssuerName { get; set; }

        [JsonPropertyName("total_shares")]
        public int TotalShares { get; set; }

        [JsonPropertyName("share_price")]
        public decimal SharePrice { get; set; }
    }

    public class CommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Operation)
                .NotEmpty();
            //.In("BUY", "SELL");

            RuleFor(x => x.IssuerName).NotEmpty();

            RuleFor(x => x.TotalShares)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.SharePrice)
               .NotEmpty()
               .GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<CreateOrderCommand, OrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var businessErrors = new List<string>();
            var issuers = new List<IssuerDto>();

            // Getting the account
            var account = await _context.Accounts
                .Include(x => x.Orders)
                .Include(x => x.Stocks)
                .FirstOrDefaultAsync(x => x.Id == request.AccountId);            

            if (account == null)
                throw new RestException(HttpStatusCode.NotFound, new { Account = "Not found" });
           
            var order = new Order
            {
                AccountId = request.AccountId,
                TimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Operation = request.Operation,
                IssuerName = request.IssuerName.Trim(),
                TotalShares = request.TotalShares,
                SharePrice = request.SharePrice
            };

            // Calculating the operation total amount 
            var grandTotal = request.SharePrice * request.TotalShares;

            // Validating business rules
            businessErrors = BusinessValidator.Validate(account, order);
                          
            if (businessErrors.Count == 0)
            {
                // Adding order
                _context.Orders.Add(order);

                // Getting the specific stock from the account
                var stock = account.Stocks
                    .Where(x => x.IssuerName == order.IssuerName)
                    .SingleOrDefault();

                if (stock != null)
                {
                    // Updating the current stock
                    stock.Quantity = string.Equals(request.Operation, "BUY") ?
                        stock.Quantity += request.TotalShares : // Adding to the stock if is buy
                        stock.Quantity -= request.TotalShares;  // Removing to the stock if is sell
                }
                else
                {
                    stock = new Stock
                    {
                        AccountId = request.AccountId,
                        IssuerName = request.IssuerName,
                        Quantity = request.TotalShares
                    };

                    _context.Stocks.Add(stock); // Creating new element in the account's stock
                }
              
                // Updating account's balance
                account.Cash = string.Equals(request.Operation, "SELL") ?
                                   account.Cash += grandTotal :
                                           account.Cash -= grandTotal;
               
                // Saving changes
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                // Mapping issuers
                foreach (var orders in account.Orders)
                    issuers.Add(_mapper.Map<Order, IssuerDto>(orders));

                if (!success)
                    throw new Exception("Problem saving changes");                
            }

            return new OrderResult
            {
                CurrentBalance = new CurrentBalance
                {
                    Cash = account.Cash,
                    Issuers = issuers
                },
                BusinessErrors = businessErrors
            };
        }       
    }
}
