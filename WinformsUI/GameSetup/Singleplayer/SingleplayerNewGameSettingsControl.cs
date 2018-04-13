namespace WinformsUI.GameSetup.Singleplayer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Client.Entities;
    using FormatConverters;
    using GameAi.Data.Restrictions;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameRestrictions;
    using GameObjectsLib.Players;
    using HelperControls;
    using HelperObjects;

    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        private readonly MyHumanPlayerControl myHumanPlayerControl;

        public event Action<Game> OnGameStarted;

        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();

            startButton.Enabled = false;

            var colorToPick = Global.PlayerColorPicker.PickAny() ?? throw new ArgumentException("All colors depleted.");
            myHumanPlayerControl = new MyHumanPlayerControl
            {
                Parent = myPlayerPanel,
                Dock = DockStyle.Fill,
                PlayerColor = colorToPick
            };
            myHumanPlayerControl.Show();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            
            aiPlayersNumberNumericUpDown.Maximum = 1;
            aiPlayerSettingsControl.PlayersLimit = 1;
            // initially there has to be selected 1 player
            aiPlayersNumberNumericUpDown.Minimum = 1;

            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit - 1;
                aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit - 1;

                startButton.Enabled = true;
            };
        }
        
        private decimal previousPlayersNumber;

        private void PlayersNumberChanged(object sender, EventArgs e)
        {
            if (previousPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = aiPlayersNumberNumericUpDown.Value - previousPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();
                    // move start button
                    startButton.Location = new Point(startButton.Location.X, startButton.Location.Y + 34);
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
            else if (previousPlayersNumber > aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = previousPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                    // move start button
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

                IList<AiPlayer> aiPlayers = aiPlayerSettingsControl.GetPlayers();

                IList<Player> players = new List<Player>();
                foreach (AiPlayer aiPlayer in aiPlayers)
                {
                    players.Add(aiPlayer);
                }

                players.Add(myHumanPlayerControl.GetPlayer());

                Game game = null;
                // generate id for the game
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    IEnumerable<SingleplayerSavedGameInfo> savedGamesEnum =
                        db.SingleplayerSavedGameInfos.AsEnumerable();
                    SingleplayerSavedGameInfo lastGame = savedGamesEnum.LastOrDefault();
                    int gameId = 1;
                    if (lastGame != null)
                    {
                        gameId = lastGame.Id + 1;
                    }

                    // get restrictions
                    var gameRestrictions = new GameObjectsRestrictionsGenerator(map, players, 2).Generate();

                    var factory = new GameFactory();
                    game = factory.CreateGame(gameId, GameType.SinglePlayer, map,
                        players, fogOfWar: fogOfWarCheckBox.Checked, objectsRestrictions: gameRestrictions);
                }

                OnGameStarted?.Invoke(game);
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
