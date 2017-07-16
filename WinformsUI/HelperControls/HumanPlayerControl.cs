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
    public partial class HumanPlayerControl : UserControl
    {
        HumanPlayer player;

        public HumanPlayerControl(User user)
        {
            InitializeComponent();

            player = new HumanPlayer(user, KnownColor.Aqua);
        }

        public HumanPlayerControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Accesses players controlling user.
        /// </summary>
        public User User
        {
            get { return player?.User; }
            set
            {
                player = new HumanPlayer(value, KnownColor.Aqua);
                playerNameLabel.Text = player.Name;
            }
        }
        

        /// <summary>
        /// Accesses player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            private set
            {
                player = new HumanPlayer(player?.User, value);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }


        /// <summary>
        /// Accesses player name.
        /// </summary>
        public string PlayerName
        {
            get { return player?.Name; }
        }

        /// <summary>
        /// Returns new instance representing the player.
        /// </summary>
        /// <returns>Player</returns>
        public Player GetPlayer()
        {
            return new HumanPlayer(User, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            if (player == null) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    PlayerColor++; // hopefully there is no upper or down limit
                    break;
                case MouseButtons.Right:
                    PlayerColor--;
                    break;
            }
        }
    }
}
