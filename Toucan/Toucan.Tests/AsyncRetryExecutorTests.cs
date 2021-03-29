
using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Toucan.Retry;

namespace Toucan.Tests
{
    [TestFixture]
    public class AsyncRetryExecutorTests
    {
        [Test]
        public async Task ShouldReturnNull()
        {
            var cancellationToken = new CancellationToken(false);

            var execute = await AsyncRetryExecutor.Execute<object>(cancellationToken, token => null, async (exception) => await OnException(exception));

            Assert.IsNull(execute);
        }

        private ValueTask<RetryStrategy> OnException(Exception exception)
        {
            return new ValueTask<RetryStrategy>(RetryStrategy.None);
        }

        [Test]
        public async Task ShouldReturnExpected()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                value++;
                return value;
            }, async (exception) => await OnException(exception));

            Assert.IsNotNull(execute);
            Assert.True(value == 2);
        }

        [Test]
        public async Task ShouldRetryStrategyAsNone_IfOnExceptionMethodIsNull()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();
                }
                while (value < 5);

                return value;
            }, onException: async exception => null);

            Assert.IsNotNull(execute);
        }

        [Test]
        public async Task ShouldGetExceptionIfValueIsTwoButRetryOneMoreTimeAndReturnExpected()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();
                }
                while (value < 5);

                return value;
            }, async exception => await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.One)));

            Assert.IsNotNull(execute);
            Assert.True(value == 5);
        }

        [Test]
        public async Task ShouldGetExceptionIfValueIsTwoAndFourButRetryOneMoreTimeAndReturnFour_BecauseRetryIsOneTime()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();

                    if (value == 4)
                        throw new Exception();
                }
                while (value < 5);

                return value;
            }, async exception => await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.One)));

            Assert.IsNotNull(execute);
            Assert.True(value == 4);
        }

        [Test]
        public async Task ShouldGetExceptionIfValueIsTwoAndFourButRetryOneMoreTimeAndReturnFour_BecauseRetryIsTwoTime()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();

                    if (value == 4)
                        throw new Exception();
                }
                while (value < 5);

                return value;
            }, async exception => await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.Two)));

            Assert.IsNotNull(execute);
            Assert.True(value == 5);
        }

        [Test]
        public async Task ShouldReturnExpectedIfThereIsNoRetryPlanForExceptionType()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();
                }
                while (value < 3);

                return value;
            }, async exception =>
            {
                if (exception is ArgumentException)
                    return await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.One));

                return await new ValueTask<RetryStrategy>(RetryStrategy.None);
            });

            Assert.IsNotNull(execute);
            Assert.True(value == 2);
        }

        [Test]
        public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionType()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new ArgumentException();
                }
                while (value < 3);

                return value;
            }, async exception =>
            {
                if (exception is ArgumentException)
                    return await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.One));

                return await new ValueTask<RetryStrategy>(RetryStrategy.None);
            });

            Assert.IsNotNull(execute);
            Assert.True(value == 3);
        }

        [Test]
        public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionTypeAndGiveUsTryCount()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var expectedTryCount = 1;
            var triedCount = 0;

            async Task OnBefore(RetryStrategy rs, int tc) => triedCount = tc;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new ArgumentException();
                }
                while (value < 3);

                return value;
            }, async exception =>
            {
                if (exception is ArgumentException)
                    return await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.One));

                return await new ValueTask<RetryStrategy>(RetryStrategy.None);
            }, OnBefore);

            Assert.IsNotNull(execute);
            Assert.True(value == 3);
            Assert.True(triedCount == expectedTryCount);
        }

        [Test]
        public async Task ShouldReturnExpectedIfThereIsRetryPlanForExceptionTypeAndGiveUsTryCountAsTwo()
        {
            var cancellationToken = new CancellationToken(false);
            var value = 1;

            var expectedTryCount = 2;
            var triedCount = 0;

            async Task OnBefore(RetryStrategy rs, int tc) => triedCount = tc;

            var execute = await AsyncRetryExecutor.Execute(cancellationToken, async token =>
            {
                do
                {
                    value++;

                    if (value == 2)
                        throw new Exception();

                    if (value == 4)
                        throw new Exception();
                }
                while (value < 5);

                return value;
            }, async exception =>
            {
                if (exception is Exception)
                    return await new ValueTask<RetryStrategy>(new RetryStrategy(RetryTimes.Two));

                return await new ValueTask<RetryStrategy>(RetryStrategy.None);
            }, OnBefore);

            Assert.IsNotNull(execute);
            Assert.True(value == 5);
            Assert.True(triedCount == expectedTryCount);
        }
    }
}