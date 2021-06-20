using FakeItEasy;
using FluentAssertions;
using Kysect.BotFramework.Core.CommandInvoking;
using Kysect.BotFramework.Core.Commands;
using Kysect.BotFramework.DefaultCommands;
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