﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Toucan.Models;

namespace Toucan.Retry.Async;

/// <summary>
/// 
/// </summary>
public static class AsyncRetryExecutor
{
    private static readonly AsyncRetryExecutorWrapper AsyncRetryExecutorWrapper;

    static AsyncRetryExecutor()
    {
        AsyncRetryExecutorWrapper = new AsyncRetryExecutorWrapper();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="action"></param>
    /// <param name="onException"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
        , Func<CancellationToken, Task<TResult>?> action
        , Func<Exception, ValueTask<RetryStrategy?>> onException)
    {
        return AsyncRetryExecutorWrapper.Execute(cancellationToken, action, onException);
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
    public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
        , Func<CancellationToken, Task<TResult>?> action
        , Func<Exception, ValueTask<RetryStrategy?>> onException
        , Func<RetryStrategy, int, ValueTask> beforeRetry)
    {
        return AsyncRetryExecutorWrapper.Execute(cancellationToken, action, onException, beforeRetry);
    }

    /// <summary>
    /// ßßß
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="action"></param>
    /// <param name="onException"></param>
    /// <param name="beforeRetry"></param>
    /// <param name="throwException"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static Task<TResult> Execute<TResult>(CancellationToken cancellationToken
        , Func<CancellationToken, Task<TResult>?> action
        , Func<Exception, ValueTask<RetryStrategy?>> onException
        , Func<RetryStrategy, int, ValueTask> beforeRetry
        , bool throwException)
    {
        return AsyncRetryExecutorWrapper.Execute(cancellationToken, action, onException, beforeRetry, throwException);
    }
}