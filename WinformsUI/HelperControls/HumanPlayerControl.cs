using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib.GameUser;
using GameObjectsLib;

namespace WinformsUI.HelperControls
{
    public partial class HumanPlayerControl : UserControl
    {
        HumanPlayer player;

        public HumanPlayerControl()
        {
            InitializeComponent();
            player = new HumanPlayer(new LocalUser(), KnownColor.Aqua);
        }
        
        void UserChanged(User user)
        {
            player = new HumanPlayer(user, PlayerColor);
            playerNameTextBox.Text = user.Name;
            switch (user.UserType)
            {
                case UserType.LocalUser:
                    playerNameTextBox.Enabled = true;
                    colorButton.Enabled = true;
                    break;
                case UserType.NetworkUser: // locks the values for rewriting
                    playerNameTextBox.Enabled = false;
                    colorButton.Enabled = false;
                    break;
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
                player = new HumanPlayer(player.User, value);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }
        
        /// <summary>
        /// Accesses player name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
            private set
            {
                player = new HumanPlayer(new LocalUser(value), PlayerColor);
                playerNameTextBox.Text = value;
            }
        }

        /// <summary>
        /// Returns new instance representing the player.
        /// </summary>
        /// <returns>Player</returns>
        public Player GetPlayer()
        {
            return new HumanPlayer(player.User, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int)PlayerColor >= 173) PlayerColor = (KnownColor)0;
                    else PlayerColor++;
                    break;
                case MouseButtons.Right:
                    if ((int)PlayerColor <= 0) PlayerColor = (KnownColor)173;
                    else PlayerColor--;
                    break;
            }
        }

        private void PlayerNameTextChanged(object sender, EventArgs e)
        {
            PlayerName = playerNameTextBox.Text;
        }
    }
}
