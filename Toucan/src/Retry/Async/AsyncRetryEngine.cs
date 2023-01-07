using System;
using System.Threading;
using System.Threading.Tasks;
using Toucan.Models;

namespace Toucan.Retry.Async;

internal static class AsyncRetryEngine
{
    internal static async Task<TResult> ImplementationExecuteAsync<TResult>(CancellationToken cancellationToken
        , Func<CancellationToken, Task<TResult>?> action
        , Func<Exception, ValueTask<RetryStrategy?>> onException
        , Func<RetryStrategy, int, ValueTask> beforeRetry
        , bool throwException = false
        , bool continueOnCapturedContext = false)
    {
        var tryCount = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            RetryStrategy? retryStrategy;

            try
            {
                var result = await action(cancellationToken)!.ConfigureAwait(continueOnCapturedContext);

                return result;
            }
            catch (Exception ex)
            {
                retryStrategy = await onException(ex) ?? RetryStrategy.None;

                var canRetry = tryCount < retryStrategy!.PermittedRetryCount;

                if (!canRetry)
                {
                    if (throwException)
                        throw;

                    return default!;
                }
            }

            if (tryCount < retryStrategy.PermittedRetryCount) tryCount++;

            await beforeRetry(retryStrategy, tryCount).ConfigureAwait(continueOnCapturedContext);

            if (retryStrategy.WaitDuration > TimeSpan.Zero)
                await Task.Delay(retryStrategy.WaitDuration, cancellationToken);
        }
    }
}