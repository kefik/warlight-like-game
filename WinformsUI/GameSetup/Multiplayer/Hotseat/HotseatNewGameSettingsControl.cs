using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;
using WinformsUI.HelperControls;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    public partial class HotseatNewGameSettingsControl : UserControl
    {
        public Func<User> GetUser;
        public Action<User> SetUser;

        readonly MyHumanPlayerControl myHumanPlayerControl;
        /// <summary>
        /// Represents number of total players that can play this given map.
        /// </summary>
        int TotalPlayersLimit
        {
            get { return mapSettingsControl.PlayersLimit; }
        }

        public HotseatNewGameSettingsControl()
        {
            InitializeComponent();

            myHumanPlayerControl = new MyHumanPlayerControl()
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


            previousAIPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        public event Action<Game> OnGameStarted;

        decimal previousAIPlayersNumber;
        private void OnNumberOfAIPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(aiPlayersNumberNumericUpDown.Value, previousHumanPlayersNumber))
            {
                aiPlayersNumberNumericUpDown.Value = previousAIPlayersNumber;
                return;
            }

            decimal difference;
            if (previousAIPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                difference = aiPlayersNumberNumericUpDown.Value - previousAIPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();
                }
            }
            else
            {
                difference = previousAIPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                }
            }
            
            previousAIPlayersNumber = aiPlayersNumberNumericUpDown.Value;
        }

        decimal previousHumanPlayersNumber;
        private void OnNumberOfHumanPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(humanPlayersNumberNumericUpDown.Value, previousAIPlayersNumber))
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

        bool DoesSumToPlayersLimit(decimal aiPlayers, decimal humanPlayers)
        {
            return aiPlayers + humanPlayers <= Math.Max(0, TotalPlayersLimit - 1);
        }

        private void Start(object sender, EventArgs e)
        {
            Map map = mapSettingsControl.GetMap();

            IList<Player> players = aiPlayerSettingsControl.GetPlayers();
            players.Add(myHumanPlayerControl.GetPlayer());

            foreach (var player in humanPlayerSettingsControl.GetPlayers())
            {
                players.Add(player);
            }

            Game game = null;

            using (var db = new UtilsDbContext())
            {
                var savedGamesEnum = db.HotseatSavedGameInfos.AsEnumerable();
                var lastGame = savedGamesEnum.LastOrDefault();
                int gameId = 1;
                if (lastGame != null) gameId = lastGame.Id + 1;

                game = Game.Create(gameId, GameType.MultiplayerHotseat, map, players);


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
