using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.GameSetup.Simulator
{
    using Client.Entities;
    using FormatConverters;
    using GameAi.Data.Restrictions;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.Players;

    public partial class SimulatorNewGameSettingsControl : UserControl
    {
        private decimal previousPlayersNumber;
        public Action<Game> OnSimulationStarted;
        
        public SimulatorNewGameSettingsControl()
        {
            InitializeComponent();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            simulatorBotSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
                simulatorBotSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            };
        }
        private void PlayersNumberChanged(object sender, EventArgs e)
        {
            if (previousPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = aiPlayersNumberNumericUpDown.Value - previousPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    simulatorBotSettingsControl.AddPlayer();
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
            else if (previousPlayersNumber > aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = previousPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    simulatorBotSettingsControl.RemovePlayer();
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
        }

        private void Start(object sender, EventArgs e)
        {
            try
            {
                Map map = mapSettingsControl.GetMap();

                IList<AiPlayer> aiPlayers = simulatorBotSettingsControl.GetPlayers();
                var players = aiPlayers.Cast<Player>().ToList();

                Restrictions restrictions = null;
                if (generateRestrictionsCheckBox.Checked)
                {
                    restrictions = new RestrictionsGenerator(map.Regions.Select(x => x.Id),
                        aiPlayers.Select(x => x.Id)).Generate();
                }
                var gameRestrictions = restrictions
                    ?.ToGameRestrictions(map, players);

                var factory = new GameFactory();
                var game = factory.CreateGame(0, GameType.Simulator, map, players, fogOfWar: fogOfWarCheckBox.Checked,
                    objectsRestrictions: gameRestrictions);
                
                OnSimulationStarted?.Invoke(game);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "One or more components required to start the game are missing! Please, reinstall the game!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
