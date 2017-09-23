namespace WinformsUI.GameSetup.Singleplayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using HelperControls;

    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        private readonly MyHumanPlayerControl myHumanPlayerControl;

        public event Action<Game> OnGameStarted;

        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();

            myHumanPlayerControl = new MyHumanPlayerControl
            {
                Parent = myPlayerPanel,
                Dock = DockStyle.Fill
            };
            myHumanPlayerControl.Show();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit - 1;
                aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit - 1;
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
                }
                previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            }
            else if (previousPlayersNumber > aiPlayersNumberNumericUpDown.Value)
            {
                decimal difference = previousPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
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

                    game = Game.Create(gameId, GameType.SinglePlayer, map, players);
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
