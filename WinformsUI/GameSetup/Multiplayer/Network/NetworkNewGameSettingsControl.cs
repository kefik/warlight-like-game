using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;
using WinformsUI.HelperControls;

namespace WinformsUI.GameSetup.Multiplayer.Network
{
    public partial class NetworkNewGameSettingsControl : UserControl
    {
        Func<User> getUser;
        public Func<User> GetUser
        {
            get { return getUser; }
            set
            {
                getUser = value;
                myPlayerControl.GetUser = value;
            }
        }

        Action<User> setUser;
        public Action<User> SetUser
        {
            get { return setUser; }
            set
            {
                setUser = value;
                myPlayerControl.SetUser = value;
                myPlayerControl.User = GetUser();
            }
        }

        readonly MyHumanPlayerControl myPlayerControl;
        /// <summary>
        /// Represents number of total players that can play this given map.
        /// </summary>
        int TotalPlayersLimit
        {
            get { return mapSettingsControl.PlayersLimit; }
        }

        public NetworkNewGameSettingsControl()
        {
            InitializeComponent();

            myPlayerControl = new MyHumanPlayerControl
            {
                Parent = myUserPanel,
                Dock = DockStyle.Fill
            };
            myPlayerControl.Show();

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

        public event Func<ICollection<AIPlayer>, string, int, Task> OnGameCreated;

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
                    humanPlayerSettingsControl.AddPlayer(new NetworkUser("")); // TODO: problem with users
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

        private void Create(object sender, EventArgs e)
        {
            // TODO: mb bug in conversion
            OnGameCreated?.Invoke((ICollection<AIPlayer>)aiPlayerSettingsControl.GetPlayers(), mapSettingsControl.Name, aiPlayerSettingsControl.PlayersCount);
        }
    }
}
