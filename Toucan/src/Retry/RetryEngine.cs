using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Toucan.Models;

namespace Toucan.Retry
{
    internal static class RetryEngine
    {
        internal static TResult ImplementationExecute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry
            , List<Func<Exception, bool>> shouldNotThrownException)
        {
            var tryCount = 0;
            try
            {
                while (true)
                {
                    RetryStrategy? retryStrategy;
                    Exception lasException;

                    try
                    {
                        var result = action(cancellationToken);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        retryStrategy = onException(ex);
                        retryStrategy = retryStrategy ?? RetryStrategy.None;

                        var canRetry = tryCount < retryStrategy.PermittedRetryCount;

                        if (!canRetry)
                        {
                            var thrown = shouldNotThrownException.Select(predicate => predicate(ex)).FirstOrDefault();

                            if (thrown)
                                throw;

                            return default(TResult);
                        }

                        lasException = ex;
                    }

                    if (tryCount < retryStrategy.PermittedRetryCount)
                    {
                        tryCount++;
                    }

                    beforeRetry(retryStrategy, tryCount, retryStrategy.WaitDuration, lasException, cancellationToken);

                    if (retryStrategy.WaitDuration > TimeSpan.Zero)
                    {
                        Thread.Sleep(retryStrategy.WaitDuration);
                    }
                }
            }
            finally
            {

            }
        }
    }
}