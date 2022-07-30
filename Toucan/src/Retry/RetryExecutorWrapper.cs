using System;
using System.Collections.Generic;
using System.Threading;
using Toucan.Models;

namespace Toucan.Retry
{
    internal class RetryExecutorWrapper
    {
        internal TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action)
        {
            var doNothingOnException = new Func<Exception, RetryStrategy?>(exception => RetryStrategy.None);
            var doNothingBeforeRetry = new Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken>((strategy, retryCount, waitDuration, lastException, token) => { });

            return Execute(cancellationToken, action, doNothingOnException, doNothingBeforeRetry);
        }

        internal static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException)
        {
            var doNothingBefore = new Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken>((strategy, retryCount, waitDuration, lastException, token) => { });

            return Execute(cancellationToken, action, onException, doNothingBefore);
        }

        internal static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry)
        {
            return Execute(cancellationToken, action, onException, beforeRetry,new List<Func<Exception, bool>>());
        }

        internal static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry
            , List<Func<Exception, bool>> shouldNotThrownException)
        {
            return RetryEngine.ImplementationExecute(cancellationToken, action, onException, beforeRetry, shouldNotThrownException);
        }
    }
}