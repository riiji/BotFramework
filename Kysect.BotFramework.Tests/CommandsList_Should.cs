using System;
using FakeItEasy;
using FluentAssertions;
using Kysect.BotFramework.Core.CommandInvoking;
using NUnit.Framework;

namespace Kysect.BotFramework.Tests
{
    public class CommandsList_Should
    {
        [SetUp]
        public void Setup()
        {
            _commands = new CommandHolder();

            _fakeCommand = A.Fake<IBotCommand>();
            A.CallTo(() => _fakeCommand.CommandName).Returns("someCommand");
        }

        private CommandHolder _commands;
        private IBotCommand _fakeCommand;
        
        [Test]
        public void RegisterCommand_Correctly()
        {
            _commands.AddCommand(_fakeCommand);
            _commands.GetCommand("someCommand").IsSuccess.Should().Be(true);
            _commands.GetCommand("someCommand").Value.Should().Be(_fakeCommand);
        }

        [Test]
        public void RegisterCommand_ShouldIgnoreSameCommands()
        {
            _commands.AddCommand(_fakeCommand);
            _commands.AddCommand(_fakeCommand);

            _commands.GetCommand("someCommand").IsSuccess.Should().Be(true);
            _commands.GetCommand("someCommand").Value.Should().Be(_fakeCommand);
        }

        [Test]
        public void RegisterCommand_ShouldGetCorrectCommand_WhenManyCommands()
        {
            A.CallTo(() => _fakeCommand.CommandName).Returns("someCommand1").Once();
            A.CallTo(() => _fakeCommand.CommandName).Returns("someCommand2").Once();
            A.CallTo(() => _fakeCommand.CommandName).Returns("someCommand3").Once();
            A.CallTo(() => _fakeCommand.CommandName).Returns("someCommand4").Once();

            _commands.AddCommand(_fakeCommand);
            _commands.AddCommand(_fakeCommand);
            _commands.AddCommand(_fakeCommand);
            _commands.AddCommand(_fakeCommand);

            _commands.GetCommand("someCommand3").IsSuccess.Should().Be(true);
            _commands.GetCommand("someCommand3").Value.Should().Be(_fakeCommand);
            
            _commands.GetCommand("someCommand1").IsSuccess.Should().Be(true);
            _commands.GetCommand("someCommand1").Value.Should().Be(_fakeCommand);
        }

        [Test]
        public void RegisterCommand_ReturnsNull_WhenGetInvalidCommand()
        {
            Action action = () =>
            {
                IBotCommand botCommand = _commands.GetCommand("someCommand").Value;
            };

            _commands.GetCommand("someCommand").IsSuccess.Should().Be(false);
            action.Should().Throw<InvalidOperationException>();
        }
    }
}