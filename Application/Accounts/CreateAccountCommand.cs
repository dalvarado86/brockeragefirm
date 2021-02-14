using Application.Orders;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts
{
    public class CreateAccountCommand : IRequest<AccountResult>
    {
        public decimal Cash { get; set; }
    }

    public class CommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Cash)
                .NotEmpty()
                .GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<CreateAccountCommand, AccountResult>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountResult> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Cash = request.Cash
            };

            account = _context.Accounts
                .Add(account).Entity;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
                return new AccountResult
                {
                    Id = account.Id,
                    Cash = account.Cash
                };

            throw new Exception("There are a problem saving changes");
        }
    }

    public class AccountResult
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        public IList<string> Issuers { get; private set; } = new List<string>();
    }
}
