using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Kysect.BotFramework.Core.Tools.Extensions;
using NUnit.Framework;

namespace Kysect.BotFramework.Tests
{
    public class WaitSafe_Should
    {
        private Func<Task> _fakeFunction;

        [SetUp]
        public void Setup()
        {
            _fakeFunction = A.Fake<Func<Task>>();

            A.CallTo(() => _fakeFunction.Invoke())
             .Invokes(() => Thread.Sleep(3000))
             .Returns(Task.FromException(new Exception()));
        }

        [Test]
        public void WaitSafe_WhenException_ShouldReturnFailedTask()
        {
            Task fakeFunctionTask = _fakeFunction.Invoke();
            fakeFunctionTask.WaitSafe();

            fakeFunctionTask.IsFaulted.Should().Be(true);
        }

        [Test]
        public void WaitSafe_WhenException_ShouldCompleted()
        {
            Task fakeFunctionTask = _fakeFunction.Invoke();
            fakeFunctionTask.WaitSafe();

            fakeFunctionTask.IsCompleted.Should().Be(true);
        }

        [Test]
        public void WaitSafe_WhenException_ShouldntCompletedSuccessfully()
        {
            Task fakeFunctionTask = _fakeFunction.Invoke();
            fakeFunctionTask.WaitSafe();

            fakeFunctionTask.IsCompletedSuccessfully.Should().Be(false);
        }
    }
}