using System;
using System.Threading;
using System.Threading.Tasks;
using Toucan.Models;

namespace Toucan.Retry.Async
{
    internal class AsyncRetryExecutorWrapper
    {
        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action)
        {
            return Execute(cancellationToken, action, exception => new ValueTask<RetryStrategy?>());
        }

        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action
            , Func<Exception, ValueTask<RetryStrategy?>> onException)
        {
            return Execute(cancellationToken,
                action,
                onException,
                (strategy,
                    count) => new ValueTask());
        }

        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
                                                            , Func<CancellationToken, Task<TResult>?> action
                                                            , Func<Exception, ValueTask<RetryStrategy?>> onException
                                                            , Func<RetryStrategy, int, ValueTask> beforeRetry)
        {
            return Execute(cancellationToken, action, onException, beforeRetry, false);
        }
        
        internal static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action
            , Func<Exception, ValueTask<RetryStrategy?>> onException
            , Func<RetryStrategy, int, ValueTask> beforeRetry
            , bool throwException
            , bool continueOnCapturedContext = false)
        {
            return AsyncRetryEngine.ImplementationExecuteAsync(cancellationToken, action, onException, beforeRetry, throwException, continueOnCapturedContext);
        }
    }
}