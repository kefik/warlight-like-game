using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;
using WinformsUI.HelperControls;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerNewGameSettingsControl : UserControl
    {
        readonly MyHumanPlayerControl myHumanPlayerControl;

        public User User
        {
            get { return myHumanPlayerControl.User; }
            set
            {
                myHumanPlayerControl.User = value;
            }
        }

        public event Action<Game> OnGameStarted;
        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();

            myHumanPlayerControl = new MyHumanPlayerControl
            {
                User = new LocalUser("Me"),
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


        decimal previousPlayersNumber;
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

                IList<Player> players = aiPlayerSettingsControl.GetPlayers();
                players.Add(myHumanPlayerControl.GetPlayer());

                Game game = null;
                // generate id for the game
                using (var db = new UtilsDbContext())
                {
                    var savedGamesEnum = db.SingleplayerSavedGameInfos.AsEnumerable();
                    var lastGame = savedGamesEnum.LastOrDefault();
                    int gameId = 1;
                    if (lastGame != null) gameId = lastGame.Id + 1;

                    game = Game.Create(gameId, GameType.SinglePlayer, map, players);
                }
                OnGameStarted?.Invoke(game);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("One or more components required to start the game are missing! Please, reinstall the game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
