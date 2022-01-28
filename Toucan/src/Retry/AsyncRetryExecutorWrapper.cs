using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toucan.Retry
{
    internal class AsyncRetryExecutorWrapper
    {
        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action)
        {
            var doNothing = new Func<Exception, Task<RetryStrategy?>>(exception => new Task<RetryStrategy?>(() => RetryStrategy.None));
            var doNothingBefore = new Func<RetryStrategy, int, Task>((strategy, count) => Task.FromResult(new Task<RetryStrategy?>(() => RetryStrategy.None)));

            return Execute(cancellationToken, action, doNothing, doNothingBefore);
        }

        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action
            , Func<Exception, Task<RetryStrategy?>> onException)
        {
            var doNothingBefore = new Func<RetryStrategy, int, Task>((strategy, count) => Task.FromResult(new Task<RetryStrategy?>(() => RetryStrategy.None)));

            return Execute(cancellationToken, action, onException, doNothingBefore);
        }

        internal Task<TResult> Execute<TResult>(CancellationToken cancellationToken
                                                            , Func<CancellationToken, Task<TResult>?> action
                                                            , Func<Exception, Task<RetryStrategy?>> onException
                                                            , Func<RetryStrategy, int, Task> beforeRetry)
        {
            return Execute(cancellationToken, action, onException, beforeRetry, false);
        }
        
        internal static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>?> action
            , Func<Exception, Task<RetryStrategy?>> onException
            , Func<RetryStrategy, int, Task> beforeRetry
            , bool throwException
            , bool continueOnCapturedContext = false)
        {
            return AsyncRetryEngine.ImplementationExecuteAsync(cancellationToken, action, onException, beforeRetry, throwException, continueOnCapturedContext);
        }
    }
}