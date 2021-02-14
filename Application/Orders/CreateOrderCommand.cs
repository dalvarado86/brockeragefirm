using Application.Exceptions;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                .NotEmpty()
                .In("BUY", "SELL");

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
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var businessErrors = new List<string>();
            var issuers = new List<IssuerDto>();

            var account = await _context.Accounts
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken: cancellationToken);

            if (account == null)
                throw new RestException(HttpStatusCode.NotFound, new { account = "Not found" });
           
            var order = new Order
            {
                AccountId = request.AccountId,
                TimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Operation = request.Operation,
                IssuerName = request.IssuerName,
                TotalShares = request.TotalShares,
                SharePrice = request.SharePrice
            };

            var grandTotal = request.SharePrice * request.TotalShares;

            businessErrors = BusinessValidator.Validate(account, order);
                          
            if (businessErrors.Count == 0)
            {
                // Adding order
                _context.Orders.Add(order);

                // Updating balance
                account.Cash = string.Equals(request.Operation, "SELL") ?
                                   account.Cash += grandTotal :
                                           account.Cash -= grandTotal;
               
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
