using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Kysect.BotFramework.Core;
using Kysect.BotFramework.Core.BotCommands;

namespace Kysect.BotFramework.Tests.Commands_Should
{
    public class PingCommand_Should
    {
        [Test]
        public void PingCommand_ShouldReturnPong()
        {
            PingCommand pingCommand = new PingCommand();
            CommandArgumentContainer fakeContainer = A.Fake<CommandArgumentContainer>();
            pingCommand.ExecuteAsync(fakeContainer).Result.IsSuccess.Should().Be(true);
        }

    }
}