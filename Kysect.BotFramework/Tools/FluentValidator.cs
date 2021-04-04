using System;
using FluentResults;

namespace Kysect.BotFramework.Tools
{
    public class FluentValidatorContext
    {
        public Result Value { get; }

        public FluentValidatorContext(Result result)
        {
            Value = result;
        }

        public FluentValidatorContext Continue(Func<Result> func)
        {
            return Value.IsSuccess
                ? new FluentValidatorContext(func())
                : new FluentValidatorContext(Value);
        }

        public FluentValidatorContext<T> Continue<T>(Func<Result<T>> func)
        {
            return Value.IsSuccess
                ? new FluentValidatorContext<T>(func())
                : new FluentValidatorContext<T>(Value);
        }
    }

    public class FluentValidatorContext<T>
    {
        public Result<T> Value { get; }

        public FluentValidatorContext(Result<T> result)
        {
            Value = result;
        }

        public FluentValidatorContext Continue(Func<T, Result> func)
        {
            return Value.IsSuccess
                ? new FluentValidatorContext(func(Value.Value))
                : new FluentValidatorContext(Value);
        }

        public FluentValidatorContext<TResult> Continue<TResult>(Func<T, Result<TResult>> func)
        {
            return Value.IsSuccess
                ? new FluentValidatorContext<TResult>(func(Value.Value))
                : new FluentValidatorContext<TResult>(Value.ToResult<TResult>());
        }
    }


    public static class FluentValidator
    {
        public static FluentValidatorContext Init(Result result)
        {
            return new FluentValidatorContext(result);
        }

        public static FluentValidatorContext<T> Init<T>(Result<T> result)
        {
            return new FluentValidatorContext<T>(result);
        }
    }
}