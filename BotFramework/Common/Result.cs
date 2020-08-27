using System;

namespace Tef.BotFramework.Common
{
    public class Result<T> : Result
    {
        private readonly T _value;

        private Result(bool isSuccess, T result, Exception exception = null, string executeMessage = null)
            : base(isSuccess, executeMessage)
        {
            _value = result;
            Exception = exception;
        }

        public static Result<T> Ok(T value, string executeMessage = null)
        {
            return new Result<T>(true, value, null, executeMessage);
        }

        public new static Result<T> Fail(string executeMessage, Exception exception)
        {
            return new Result<T>(true, default, exception, executeMessage);
        }

        public T Value => IsSuccess ? _value : throw new AggregateException(Exception);
    }

    public class Result
    {
        protected Result(bool isSuccess, string executeMessage = null)
        {
            IsSuccess = isSuccess;
            _executeMessage = executeMessage;
        }

        public static Result Ok(string executeMessage = null)
        {
            return new Result(true, executeMessage);
        }

        public static Result Fail(string message, Exception exception = null)
        {
            return new Result(false, message) { Exception = exception };
        }

        private readonly string _executeMessage;

        public Exception Exception { get; protected set; }
        public bool IsSuccess { get; }

        public string ExecuteMessage
        {
            get
            {
                if (Exception != null) return $"{_executeMessage}" + Environment.NewLine + Exception.Message;
                return _executeMessage;
            }
        }
    }
}