using Application.Exceptions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orders
{
    public class CreateOrderCommand : IRequest
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

    public class Handler : IRequestHandler<CreateOrderCommand>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public class CommandValidator : AbstractValidator<CreateOrderCommand>
        {
            public CommandValidator()
            {
                //RuleFor(x => x.AccountId).NotEmpty();
                RuleFor(x => x.Operation).NotEmpty();
                RuleFor(x => x.IssuerName).NotEmpty();
                RuleFor(x => x.TotalShares)
                    .NotEmpty()
                    .GreaterThan(0);
                RuleFor(x => x.SharePrice)
                   .NotEmpty()
                   .GreaterThan(0);
            }
        }

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {            
            var account = await _context.Accounts.FindAsync(request.AccountId);

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

            _context.Orders.Add(order);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
                return Unit.Value;

            throw new Exception("Problem saving changes");
        }
    }
}
