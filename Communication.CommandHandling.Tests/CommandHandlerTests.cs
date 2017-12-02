namespace Communication.CommandHandling.Tests
{
    using System;
    using NUnit.Framework;
    using Tokens.Settings;

    /// <summary>
    /// Test expects working translator.
    /// </summary>
    [TestFixture]
    public class CommandHandlerTests
    {
        private CommandHandler commandHandler;

        [SetUp]
        public void SetUp()
        {
            commandHandler = new CommandHandler();
        }

        [Test]
        public void SetupTimeBankTest()
        {
            var commandToken = new TimeBankToken(new TimeSpan(0, 0, 0, 0, milliseconds: 10000));

            var token = commandHandler.Execute(commandToken);

            Assert.AreEqual(null, token);
            Assert.AreEqual(10000, commandToken.TimeBankInterval.Value.TotalMilliseconds);
        }

        [Test]
        public void SetupTimeOutTest()
        {
            var commandToken = new TimePerMoveToken(new TimeSpan(0, 0, 0, 0, milliseconds: 500));

            var token = commandHandler.Execute(commandToken);

            Assert.AreEqual(null, token);
            Assert.AreEqual(500, commandHandler.TimePerMove.Value.TotalMilliseconds);
        }
    }
}