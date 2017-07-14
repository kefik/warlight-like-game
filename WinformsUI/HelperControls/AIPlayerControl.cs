using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConquestObjectsLib;

namespace WinformsUI.HelperControls
{
    public partial class AIPlayerControl : UserControl
    {
        AIPlayer player;
        public AIPlayerControl(string uniqueName)
        {
            InitializeComponent();

            this.player = new AIPlayer(Difficulty.Medium, "PC1", KnownColor.Blue);

            difficultyComboBox.SelectedIndex = (int) player.Difficulty;
            colorButton.BackColor = Color.FromKnownColor(player.Color);
        }

        public AIPlayerControl(AIPlayer player)
        {
            InitializeComponent();

            this.player = player;
        }
        
        /// <summary>
        /// Accesses AI player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            private set
            {
                player = new AIPlayer(player.Difficulty, player.Name, value);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }

        /// <summary>
        /// Accesses AI player difficulty.
        /// </summary>
        public Difficulty PlayerDifficulty
        {
            get { return player.Difficulty; }
            private set
            {
                player = new AIPlayer(value, player.Name, player.Color);
                difficultyComboBox.SelectedIndex = (int)player.Difficulty;
            }

        }

        /// <summary>
        /// Accesses AI player name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
            private set
            {
                player = new AIPlayer(player.Difficulty, value, player.Color);
                aiNameLabel.Text = value;
            }
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PlayerColor++;
                    break;
                case MouseButtons.Right: // TODO: doesnt work for some reason
                    PlayerColor--;
                    break;
            }
        }
    }
}
