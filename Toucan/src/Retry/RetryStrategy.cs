using System;

namespace Toucan.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public class RetryStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        public int PermittedRetryCount { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan WaitDuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryTimes"></param>
        public RetryStrategy(RetryTimes retryTimes) : this(retryTimes, TimeSpan.Zero)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryTimes"></param>
        /// <param name="waitDuration"></param>
        private RetryStrategy(RetryTimes retryTimes, TimeSpan waitDuration)
        {
            PermittedRetryCount = (int)retryTimes;
            WaitDuration = waitDuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly RetryStrategy? None = new RetryStrategy(RetryTimes.None);
    }
}