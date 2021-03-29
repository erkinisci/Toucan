using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toucan.Retry
{
    internal class AsyncRetryEngine
    {
        internal static async Task<TResult> ImplementationExecuteAsync<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, Task<TResult>> action
            , Func<Exception, Task<RetryStrategy>> onException
            , Func<RetryStrategy, int, Task> beforeRetry
            , bool throwException = false
            , bool continueOnCapturedContext = false)
        {
            var tryCount = 0;
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    RetryStrategy retryStrategy;

                    try
                    {
                        var result = await action(cancellationToken).ConfigureAwait(continueOnCapturedContext);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        retryStrategy = await onException(ex);

                        retryStrategy ??= RetryStrategy.None;

                        var canRetry = tryCount < retryStrategy.PermittedRetryCount;

                        if (!canRetry)
                        {
                            if (throwException)
                                throw;

                            return default(TResult);
                        }
                    }

                    if (tryCount < retryStrategy.PermittedRetryCount)
                    {
                        tryCount++;
                    }

                    await beforeRetry(retryStrategy, tryCount).ConfigureAwait(continueOnCapturedContext);

                    if (retryStrategy.WaitDuration > TimeSpan.Zero)
                    {
                        await Task.Delay(retryStrategy.WaitDuration, cancellationToken);
                    }
                }
            }
            finally
            {

            }
        }
    }
}