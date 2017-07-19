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

            player = new HumanPlayer(new LocalUser(""), KnownColor.Aqua);
            UserChanged(MyUser);
        }
        /// <summary>
        /// Accesses players controlling user.
        /// </summary>
        public User MyUser
        {
            get { return player.User; }
            set
            {
                if (value == null) throw new ArgumentException();

                UserChanged(value);
                player = new HumanPlayer(value, player.Color);
            }
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
        }

        /// <summary>
        /// Returns new instance representing the player.
        /// </summary>
        /// <returns>Player</returns>
        public Player GetPlayer()
        {
            return new HumanPlayer(MyUser, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
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
