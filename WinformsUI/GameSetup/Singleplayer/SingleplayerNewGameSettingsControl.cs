using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameMap;
using ConquestObjectsLib.GameUser;
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

        public event Action<ConquestObjectsLib.Game.Game> OnGameStarted;
        public SingleplayerNewGameSettingsControl()
        {
            InitializeComponent();
            
            myHumanPlayerControl = new MyHumanPlayerControl()
            {
                User = new LocalUser("Me"),
                Parent = myPlayerPanel,
                Dock = DockStyle.Fill,
            };
            myHumanPlayerControl.Show();

            previousPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit;
            aiPlayerSettingsControl.PlayersLimit = mapSettingsControl.PlayersLimit;
            // when the map is chosen, update maximum values
            mapSettingsControl.OnMapChosen += (o, e) =>
            {
                this.aiPlayersNumberNumericUpDown.Maximum = mapSettingsControl.PlayersLimit - 1;
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
            // TODO
        }
    }
}
