using System;
using System.Collections.Generic;
using System.Threading;
using Toucan.Models;

namespace Toucan.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public class RetryExecutor
    {
        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException)
        {
            return RetryExecutorWrapper.Execute(cancellationToken, action, onException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        /// <param name="beforeRetry"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry)
        {
            return RetryExecutorWrapper.Execute(cancellationToken, action, onException, beforeRetry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        /// <param name="beforeRetry"></param>
        /// <param name="shouldNotThrownException"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Execute<TResult>(CancellationToken cancellationToken
            , Func<CancellationToken, TResult> action
            , Func<Exception, RetryStrategy?> onException
            , Action<RetryStrategy, int, TimeSpan, Exception, CancellationToken> beforeRetry
            , List<Func<Exception, bool>> shouldNotThrownException)
        {
            return RetryExecutorWrapper.Execute(cancellationToken, action, onException, beforeRetry, shouldNotThrownException);
        }
    }
}