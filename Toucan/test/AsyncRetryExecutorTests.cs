using System;
using System.Threading;
using System.Threading.Tasks;
using Toucan.Enums;
using Toucan.Models;
using Toucan.Retry.Async;
using Xunit;

namespace Toucan.Tests;

public class AsyncRetryExecutorTests
{
    [Fact]
    public async Task ShouldReturnNull()
    {
        var cancellationToken = new CancellationToken(false);

        var execute =
            await AsyncRetryExecutor.Execute<object>(cancellationToken, _ => null, async _ => await OnException());

        Assert.Null(execute);
    }

    private static ValueTask<RetryStrategy?> OnException()
    {
        return new ValueTask<RetryStrategy?>(RetryStrategy.None);
    }

    [Fact]
    public async Task ShouldReturnExpected()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            value++;
            return Task.FromResult(value);
        }, async _ => await OnException());
        Assert.True(value == 2);
    }

    [Fact]
    public async Task ShouldRetryStrategyAsNone_IfOnExceptionMethodIsNull()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                if (value == 2)
                    throw new Exception();
            } while (value < 5);

            return Task.FromResult(value);
        }, _ => new ValueTask<RetryStrategy?>(RetryStrategy.None));
    }

    [Fact]
    public async Task ShouldGetExceptionIfValueIsTwoButRetryOneMoreTimeAndReturnExpected()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                if (value == 2)
                    throw new Exception();
            } while (value < 5);

            return Task.FromResult(value);
        }, async _ => await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.One)));

        Assert.True(value == 5);
    }

    [Fact]
    public async Task ShouldGetExceptionIfValueIsTwoAndFourButRetryOneMoreTimeAndReturnFour_BecauseRetryIsOneTime()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                switch (value)
                {
                    case 2:
                        throw new Exception();
                    case 4:
                        throw new Exception();
                }
            } while (value < 5);

            return Task.FromResult(value);
        }, async _ => await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.One)));

        Assert.True(value == 4);
    }

    [Fact]
    public async Task ShouldGetExceptionIfValueIsTwoAndFourButRetryOneMoreTimeAndReturnFour_BecauseRetryIsTwoTime()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                switch (value)
                {
                    case 2:
                        throw new Exception();
                    case 4:
                        throw new Exception();
                }
            } while (value < 5);

            return Task.FromResult(value);
        }, async _ => await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.Two)));

        Assert.True(value == 5);
    }

    [Fact]
    public async Task ShouldReturnExpectedIfThereIsNoRetryPlanForExceptionType()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                if (value == 2)
                    throw new Exception();
            } while (value < 3);

            return Task.FromResult(value);
        }, async exception =>
        {
            if (exception is ArgumentException)
                return await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.One));

            return await new ValueTask<RetryStrategy?>(RetryStrategy.None);
        });

        Assert.True(value == 2);
    }

    [Fact]
    public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionType()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                if (value == 2)
                    throw new ArgumentException();
            } while (value < 3);

            return Task.FromResult(value);
        }, async exception =>
        {
            if (exception is ArgumentException)
                return await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.One));

            return await new ValueTask<RetryStrategy?>(RetryStrategy.None);
        });

        Assert.True(value == 3);
    }

    [Fact]
    public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionTypeAndGiveUsTryCount()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        const int expectedTryCount = 1;
        var triedCount = 0;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
        {
            do
            {
                value++;

                if (value == 2)
                    throw new ArgumentException();
            } while (value < 3);

            return Task.FromResult(value);
        }, async exception =>
        {
            if (exception is ArgumentException)
                return await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.One));

            return await new ValueTask<RetryStrategy?>(RetryStrategy.None);
        }, (_, tryCount) =>
        {
            triedCount = tryCount;

            return new ValueTask();
        });

        Assert.True(value == 3);
        Assert.True(triedCount == expectedTryCount);
    }

    [Fact]
    public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionTypeAndGiveUsTryCountAsTwo()
    {
        var cancellationToken = new CancellationToken(false);
        var value = 1;

        await AsyncRetryExecutor.Execute(cancellationToken, _ =>
            {
                do
                {
                    value++;

                    switch (value)
                    {
                        case 2:
                            throw new Exception();
                        case 4:
                            throw new Exception();
                    }
                } while (value < 5);

                return Task.FromResult(value);
            }, async _ => await new ValueTask<RetryStrategy?>(new RetryStrategy(RetryTimes.Two))
            , (_, _) => new ValueTask());

        Assert.True(value == 5);
    }
}