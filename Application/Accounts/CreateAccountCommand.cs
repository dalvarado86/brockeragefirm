using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts
{
    public class CreateAccountCommand : IRequest<Account>
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

        public class Handler : IRequestHandler<CreateAccountCommand, Account>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
            {
                var account = new Account
                {
                    Cash = request.Cash
                };

                account = _context.Accounts.Add(account).Entity;

                 var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                if(success)
                    return account;

                throw new Exception("There are a problem saving changes");
            }
        }
    }
}
