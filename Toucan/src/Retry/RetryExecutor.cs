using System;
using System.Collections.Generic;
using System.Threading;
using Toucan.Models;

namespace Toucan.Retry
{
    public class RetryExecutor
    {
        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException)
        {
            return new RetryExecutorWrapper().Execute(cancellationToken, action, onException);
        }

        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry)
        {
            return new RetryExecutorWrapper().Execute(cancellationToken, action, onException, beforeRetry);
        }

        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry
            , List<Func<Exception, bool>> shouldNotThrownException)
        {
            return new RetryExecutorWrapper().Execute(cancellationToken, action, onException, beforeRetry, shouldNotThrownException);
        }
    }
}