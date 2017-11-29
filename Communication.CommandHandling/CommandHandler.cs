namespace Communication.CommandHandling
{
    using System;
    using GameAi;
    using Shared;
    using Tokens.Settings;

    internal class CommandHandler : ICommandHandler
    {
        public TimeSpan? TimeBank { get; private set; }

        public ICommandToken Execute(ICommandToken commandToken)
        {
            switch (commandToken.CommandTokenType)
            {
                case CommandTokenType.SetupSuperRegions:
                    break;
                case CommandTokenType.SetupRegions:
                    break;
                case CommandTokenType.SetupNeighbours:
                    break;
                case CommandTokenType.SetupWastelands:
                    break;
                case CommandTokenType.SettingsTimeBank:
                    Execute((TimeBankToken)commandToken);
                    return null;
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
                case CommandTokenType.PickStartingRegionsRequest:
                    break;
                case CommandTokenType.PlaceArmiesRequest:
                    break;
                case CommandTokenType.PlaceArmiesResponse:
                    break;
                case CommandTokenType.AttackRequest:
                    break;
                case CommandTokenType.AttackResponse:
                    break;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        private void Execute(TimeBankToken timeBankToken)
        {
            TimeBank = timeBankToken.TimeBankInterval;
        }
    }
}