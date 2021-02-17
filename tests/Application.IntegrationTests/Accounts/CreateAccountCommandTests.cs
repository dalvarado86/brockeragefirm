using Application.Accounts;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Application.IntegrationTests.Accounts
{
    using static Testing;

    public class CreateAccountCommandTests : TestBase
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new CreateAccountCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<Exception>();
        }

        [Test]
        public async Task ShouldCreateNewAccount()
        {
            await RunAsTestUserAsync();

            var command = new CreateAccountCommand
            {
                Cash = 1000
            };

            var account = await SendAsync(command);

            account.Should().NotBeNull();
            account.Cash.Should().Be(command.Cash);
            account.Issuers.Count.Should().Be(0);
        }
    }
}
