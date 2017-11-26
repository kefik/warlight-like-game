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
                case CommandTokenType.SetupSuperRegions:
                    break;
                case CommandTokenType.SetupRegions:
                    break;
                case CommandTokenType.SetupNeighbours:
                    break;
                case CommandTokenType.SetupWastelands:
                    break;
                case CommandTokenType.SettingsTimePerMove:
                    break;
                case CommandTokenType.SettingsMaxRounds:
                    break;
                case CommandTokenType.SettingsBot:
                    break;
                case CommandTokenType.SettingsStartingArmies:
                    break;
                case CommandTokenType.SettingsStartingRegions:
                    break;
                case CommandTokenType.SettingsStartingPickAmount:
                    break;
                case CommandTokenType.UpdateMap:
                    break;
                case CommandTokenType.OpponentMoves:
                    break;
                case CommandTokenType.PickStartingRegion:
                    break;
                case CommandTokenType.PlaceArmiesRequest:
                    break;
                case CommandTokenType.PlaceArmiesResponse:
                    break;
                case CommandTokenType.AttackRequest:
                    break;
                case CommandTokenType.AttackReponse:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            throw new NotImplementedException();
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