using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tef.BotFramework.Abstractions;
using Tef.BotFramework.BotCommands;
using Tef.BotFramework.Common;

namespace Tef.BotFramework.Tests.Commands_Should
{
    public class PingCommand_Should
    {
        [Test]
        public void PingCommand_ShouldReturnPong()
        {
            PingCommand pingCommand = new PingCommand();
            CommandArgumentContainer fakeContainer = A.Fake<CommandArgumentContainer>();
            pingCommand.Execute(fakeContainer).IsSuccess.Should().Be(true);
        }

    }
}