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
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;

    public partial class SimulatorNewGameSettingsControl : UserControl
    {
        private decimal previousPlayersNumber;
        public Action<Game> OnSimulationStarted;
        
        public SimulatorNewGameSettingsControl()
        {
            InitializeComponent();

            startButton.Enabled = false;

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = 2;
            simulatorBotSettingsControl.PlayersLimit = 2;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
                simulatorBotSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;

                startButton.Enabled = true;
            };

            // TODO: solve generate restrictions
            generateRestrictionsCheckBox.Enabled = false;
            aiPlayersNumberNumericUpDown.Minimum = 2;
        }

        private void PlayersNumberChanged(object sender, EventArgs e)
        {
            if (previousPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = aiPlayersNumberNumericUpDown.Value - previousPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    simulatorBotSettingsControl.AddPlayer();

                    startButton.Location = new Point(startButton.Location.X, startButton.Location.Y + 34);
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
            else if (previousPlayersNumber > aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = previousPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    simulatorBotSettingsControl.RemovePlayer();

                    startButton.Location = new Point(startButton.Location.X, startButton.Location.Y - 34);
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

                GameObjectsRestrictions restrictions = null;
                if (generateRestrictionsCheckBox.Checked)
                {
                    restrictions = new GameObjectsRestrictionsGenerator(map, players, 2).Generate();
                }
                else
                {
                    restrictions = new GameObjectsRestrictions()
                    {
                        GameBeginningRestrictions = aiPlayers
                            .Select(x => new GameObjectsBeginningRestriction()
                            {
                                Player = x,
                                RegionsToChooseCount = 2, // TODO: not fix
                                RegionsPlayersCanChoose = map.Regions
                            }).ToList()
                    };
                }

                Game game = null;
                // generate id for the game
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    IEnumerable<SimulationRecord> savedGamesEnum =
                        db.SimulationRecords.AsEnumerable();
                    SimulationRecord lastGame = savedGamesEnum.LastOrDefault();
                    int gameId = 1;
                    if (lastGame != null)
                    {
                        gameId = lastGame.Id + 1;
                    }

                    // get restrictions
                    var gameRestrictions = new GameObjectsRestrictionsGenerator(map, players, 2).Generate();

                    var factory = new GameFactory();
                    game = factory.CreateGame(gameId, GameType.Simulator, map,
                        players, fogOfWar: fogOfWarCheckBox.Checked, objectsRestrictions: gameRestrictions);
                }

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
