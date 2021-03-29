using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toucan.Retry
{
    public class AsyncRetryExecutor
    {
        public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>> action
            , Func<Exception, Task<RetryStrategy>> onException)
        {
            return new AsyncRetryExecutorWrapper().Execute(cancellationToken, action, onException);
        }

        public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>> action
            , Func<Exception, Task<RetryStrategy>> onException
            , Func<RetryStrategy, int, Task> beforeRetry)
        {
            return new AsyncRetryExecutorWrapper().Execute(cancellationToken, action, onException, beforeRetry);
        }

        public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>> action
            , Func<Exception, Task<RetryStrategy>> onException
            , Func<RetryStrategy, int, Task> beforeRetry
            , bool throwException)
        {
            return new AsyncRetryExecutorWrapper().Execute(cancellationToken, action, onException, beforeRetry, throwException);
        }
    }
}