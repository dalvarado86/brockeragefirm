using Application.Accounts.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts
{
    /// <summary>
    /// GetAccountDetailsQuery.
    /// </summary>
    public class GetAccountDetailsQuery : IRequest<AccountDetailsDto>
    {
        public int AccountId { get; set; }
    }

    /// <summary>
    /// GetStocksByAccountQueryHandler.
    /// </summary>
    public class GetStocksByAccountQueryHandler : IRequestHandler<GetAccountDetailsQuery, AccountDetailsDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStocksByAccountQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStocksByAccountQueryHandler"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public GetStocksByAccountQueryHandler(
            IApplicationDbContext context, 
            IMapper mapper, 
            ILogger<GetStocksByAccountQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<AccountDetailsDto> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {            
            var account = await _context.Accounts
                .Include(x => x.Stocks)
                .Include(x => x.Orders)
                .Include(x => x.User)
                .Where(x => x.Id == request.AccountId)
                .SingleOrDefaultAsync();

            if (account == null)
            {
                _logger.LogInformation($"The account '{request.AccountId}' not found");
                throw new RestException(HttpStatusCode.NotFound, new { Account = "Not found" });
            }

            
            var accountDto = new AccountDetailsDto
            {
                AccountHolderUser = account.User.UserName,
                Cash = account.Cash,
                Orders = _mapper.Map<List<Order>, List<OrderDto>>((List<Order>)account.Orders),
                Stock = _mapper.Map<List<Stock>, List<StockDto>>((List<Stock>)account.Stocks)
            };

            _logger.LogInformation($"The account details: {accountDto}");

            return accountDto;
        }
    }
}
