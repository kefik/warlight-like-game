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
using ConquestObjectsLib.GameUser;

namespace WinformsUI.HelperControls
{
    public partial class MyHumanPlayerControl : UserControl
    {
        public MyHumanPlayerControl()
        {
            InitializeComponent();

            // initialize
            user = new LocalUser("Me");
            playerNameTextBox.Text = user.Name;
            playerNameTextBox.Enabled = true;
        }

        User user;
        /// <summary>
        /// Represents clients user. Cannot be null.
        /// </summary>
        public User User
        {
            get { return user; }
            set
            {
                if (value == null) throw new ArgumentException();
                switch (value.UserType)
                {
                    case UserType.LocalUser:
                        playerNameTextBox.Enabled = true;
                        break;
                    case UserType.MyNetworkUser:
                    case UserType.NetworkUser:
                        playerNameTextBox.Enabled = false;
                        break;
                }
                user = value;
                playerNameTextBox.Text = user.Name;
            }
        }

        KnownColor playerColor;
        /// <summary>
        /// Represents clients color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return playerColor; }
            private set
            {
                playerColor = value;
                colorButton.BackColor = System.Drawing.Color.FromKnownColor(PlayerColor);
            }
        }

        /// <summary>
        /// Represents name of player this control is representing.
        /// </summary>
        public string PlayerName
        {
            get { return user.Name; }
            private set
            {
                user.Name = value;
                playerNameTextBox.Text = value;
            }
        }


        /// <summary>
        /// Creates and returns new instance of player represented by this control.
        /// </summary>
        /// <returns>New instance of the player represented by this control.</returns>
        public Player GetPlayer()
        {
            return new HumanPlayer(User, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PlayerColor++; // TODO: fix so it doesnt overflow or underflow
                    break;
                case MouseButtons.Right:
                    PlayerColor--;
                    break;
            }
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            switch (User.UserType)
            {
                case UserType.LocalUser:
                    user.Name = Name;
                    break;
                case UserType.NetworkUser:
                    playerNameTextBox.Text = PlayerName;
                    break;
            }
        }
    }
}
