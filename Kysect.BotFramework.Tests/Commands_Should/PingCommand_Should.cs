using FakeItEasy;
using FluentAssertions;
using Kysect.BotFramework.Commands;
using NUnit.Framework;
using Kysect.BotFramework.Core.CommandInvoking;

namespace Kysect.BotFramework.Tests.Commands_Should
{
    public class PingCommand_Should
    {
        [Test]
        public void PingCommand_ShouldReturnPong()
        {
            PingCommand pingCommand = new PingCommand();
            CommandArgumentContainer fakeContainer = A.Fake<CommandArgumentContainer>();
            pingCommand.Execute(fakeContainer).Result.IsSuccess.Should().Be(true);
        }

    }
}