namespace Communication.CommandHandling.Tokens
{
    using System;
    using Settings;
    using Shared;

    public class CommandTokenFactory
    {
        public ICommandToken Create(CommandTokenType type, params object[] parameters)
        {
            switch (type)
            {
                case CommandTokenType.SettingsTimeBank:
                    return CreateSettingsTimeBank(parameters);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal TimeBankToken CreateSettingsTimeBank(params object[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new ArgumentException($"{nameof(TimeBankToken)} can accept only 1 parameter.");
            }

            TimeBankToken token = new TimeBankToken((int)parameters[0]);

            return token;
        }
    }
}