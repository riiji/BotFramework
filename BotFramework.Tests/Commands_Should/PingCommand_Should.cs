using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tef.BotFramework.Core;
using Tef.BotFramework.Core.BotCommands;

namespace Tef.BotFramework.Tests.Commands_Should
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