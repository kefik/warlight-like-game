namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using GameObjectsLib.GameUser;
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

            myHumanPlayerControl = new MyHumanPlayerControl
            {
                Parent = myUserPanel,
                Dock = DockStyle.Fill
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
            };
            // initialize
            int maxPlayers = Math.Max(0, TotalPlayersLimit - 1);
            aiPlayersNumberNumericUpDown.Maximum = maxPlayers;
            humanPlayersNumberNumericUpDown.Maximum = maxPlayers;


            aiPlayerSettingsControl.PlayersLimit = maxPlayers;
            humanPlayerSettingsControl.PlayersLimit = maxPlayers;


            previousAiPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
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
                }
            }
            else
            {
                difference = previousAiPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                }
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
                }
            }
            else
            {
                difference = previousHumanPlayersNumber - humanPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    humanPlayerSettingsControl.RemovePlayer(); // TODO: problem with users
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

                game = Game.Create(gameId, GameType.MultiplayerHotseat, map, players, fogOfWar: true);


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
