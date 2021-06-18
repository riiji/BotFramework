using FakeItEasy;
using FluentAssertions;
using Kysect.BotFramework.Commands;
using Kysect.BotFramework.Core.CommandInvoking;
using NUnit.Framework;

namespace Kysect.BotFramework.Tests.Commands_Should
{
    public class PingCommand_Should
    {
        [Test]
        public void PingCommand_ShouldReturnPong()
        {
            var pingCommand = new PingCommand();
            var fakeContainer = A.Fake<CommandContainer>();
            pingCommand.Execute(fakeContainer).Result.IsSuccess.Should().Be(true);
        }
    }
}