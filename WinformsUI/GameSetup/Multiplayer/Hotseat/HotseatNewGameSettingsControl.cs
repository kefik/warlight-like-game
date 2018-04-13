namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;
    using HelperControls;

    public partial class HotseatNewGameSettingsControl : UserControl
    {
        private readonly MyHumanPlayerControl myHumanPlayerControl;

        /// <summary>
        ///     Represents number of total players that can play this given map.
        /// </summary>
        private int TotalPlayersLimit
        {
            get { return mapSettingsControl.PlayersLimit; }
        }

        public HotseatNewGameSettingsControl()
        {
            InitializeComponent();

            startButton.Enabled = false;

            var colorToPick = Global.PlayerColorPicker.PickAny() ?? throw new ArgumentException("All colors depleted.");
            myHumanPlayerControl = new MyHumanPlayerControl
            {
                Parent = myUserPanel,
                Dock = DockStyle.Fill,
                PlayerColor = colorToPick
            };
            myHumanPlayerControl.Show();

            // TODO: get the code better
            mapSettingsControl.OnMapChosen += (sender, args) =>
            {
                int maxOtherPlayers = Math.Max(0, TotalPlayersLimit - 1);

                aiPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;
                humanPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;

                aiPlayerSettingsControl.PlayersLimit = maxOtherPlayers;
                humanPlayerSettingsControl.PlayersLimit = maxOtherPlayers;

                startButton.Enabled = true;
            };
            // initialize
            int maxPlayers = Math.Max(0, TotalPlayersLimit - 1);
            aiPlayersNumberNumericUpDown.Maximum = maxPlayers;
            humanPlayersNumberNumericUpDown.Maximum = maxPlayers;
            
            aiPlayerSettingsControl.PlayersLimit = maxPlayers;
            humanPlayerSettingsControl.PlayersLimit = maxPlayers;
            
            previousAiPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;

            // in hotseat there has to be at least one non AI opponent
            humanPlayersNumberNumericUpDown.Minimum = 1;
            aiPlayersLabel.Visible = false;
        }

        public event Action<Game> OnGameStarted;

        private decimal previousAiPlayersNumber;

        private void OnNumberOfAiPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(aiPlayersNumberNumericUpDown.Value, previousHumanPlayersNumber))
            {
                aiPlayersNumberNumericUpDown.Value = previousAiPlayersNumber;
                return;
            }

            decimal difference;
            if (previousAiPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                difference = aiPlayersNumberNumericUpDown.Value - previousAiPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();

                    // move start button
                    startButton.Location = new Point(startButton.Location.X, startButton.Location.Y + 34);
                }
            }
            else
            {
                difference = previousAiPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();

                    // move start button
                    startButton.Location = new Point(startButton.Location.X, startButton.Location.Y - 34);
                }
            }

            if (aiPlayersNumberNumericUpDown.Value == 0)
            {
                aiPlayersLabel.Visible = false;
            }
            else
            {
                aiPlayersLabel.Visible = true;
            }

            previousAiPlayersNumber = aiPlayersNumberNumericUpDown.Value;
        }

        private decimal previousHumanPlayersNumber;

        private void OnNumberOfHumanPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(humanPlayersNumberNumericUpDown.Value, previousAiPlayersNumber))
            {
                humanPlayersNumberNumericUpDown.Value = previousHumanPlayersNumber;
                return;
            }

            decimal difference;
            if (previousHumanPlayersNumber < humanPlayersNumberNumericUpDown.Value)
            {
                difference = humanPlayersNumberNumericUpDown.Value - previousHumanPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    humanPlayerSettingsControl.AddPlayer(new LocalUser("")); // TODO: problem with users

                    // move start button
                    aiPlayersPanel.Location = new Point(aiPlayersPanel.Location.X, aiPlayersPanel.Location.Y + 38);
                }
            }
            else
            {
                difference = previousHumanPlayersNumber - humanPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    humanPlayerSettingsControl.RemovePlayer(); // TODO: problem with users

                    aiPlayersPanel.Location = new Point(aiPlayersPanel.Location.X, aiPlayersPanel.Location.Y - 38);
                }
            }

            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        private bool DoesSumToPlayersLimit(decimal aiPlayers, decimal humanPlayers)
        {
            return aiPlayers + humanPlayers <= Math.Max(0, TotalPlayersLimit - 1);
        }

        private void Start(object sender, EventArgs e)
        {
            Map map = mapSettingsControl.GetMap();

            if (map == null)
            {
                MessageBox.Show(
                    "Choose a map before starting the game.");
                return;
            }

            IList<AiPlayer> aiPlayers = aiPlayerSettingsControl.GetPlayers();

            IList<Player> players = new List<Player>();
            foreach (AiPlayer aiPlayer in aiPlayers)
            {
                players.Add(aiPlayer);
            }

            players.Add(myHumanPlayerControl.GetPlayer());

            foreach (Player player in humanPlayerSettingsControl.GetPlayers())
            {
                players.Add(player);
            }

            Game game = null;

            using (UtilsDbContext db = new UtilsDbContext())
            {
                IEnumerable<HotseatSavedGameInfo> savedGamesEnum = db.HotseatSavedGameInfos.ToList();
                HotseatSavedGameInfo lastGame = savedGamesEnum.LastOrDefault();
                int gameId = 1;
                if (lastGame != null)
                {
                    gameId = lastGame.Id + 1;
                }

                var factory = new GameFactory();
                game = factory.CreateGame(gameId, GameType.MultiplayerHotseat,
                    map, players, fogOfWar: fogOfWarCheckBox.Checked, objectsRestrictions: null);


                // TEST
                /*NetworkObjectWrapper wrapper = new NetworkObjectWrapper<Game>() {TypedValue = game};
                using (var ms = new MemoryStream())
                {
                    wrapper.Serialize(ms);

                    ms.Position = 0;

                    var obj = NetworkObjectWrapper.Deserialize(ms).Value;
                }*/
                // END TEST
            }

            OnGameStarted?.Invoke(game);
        }
    }
}
