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
            player = new HumanPlayer(new LocalUser("Me"), KnownColor.Red);

            playerNameTextBox.Text = User.Name;
            playerNameTextBox.Enabled = true;
        }

        HumanPlayer player;

        /// <summary>
        /// Represents clients user. Cannot be null.
        /// </summary>
        public User User
        {
            get { return player.User; }
            set
            {
                if (value == null) throw new ArgumentException();
                UserChanged(value);
            }
        }
        /// <summary>
        /// Represents clients color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            set
            {
                player = new HumanPlayer(User, value);
                colorButton.BackColor = System.Drawing.Color.FromKnownColor(PlayerColor);
            }
        }
        /// <summary>
        /// Represents players name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
        }
        
        /// <summary>
        /// Handles user change. Locks appropriate controls reacting to the new type of user.
        /// </summary>
        /// <param name="newUser">New user</param>
        void UserChanged(User newUser)
        {
            player = new HumanPlayer(newUser, PlayerColor);
            playerNameTextBox.Text = User.Name;
            switch (User.UserType)
            {
                case UserType.LocalUser:
                    playerNameTextBox.Enabled = true;
                    break;
                case UserType.NetworkUser:
                    playerNameTextBox.Enabled = false;
                    break;
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
                    player = new HumanPlayer(new LocalUser(playerNameTextBox.Text), PlayerColor);
                    break;
                case UserType.NetworkUser:
                    playerNameTextBox.Text = PlayerName;
                    break;
            }
        }
    }
}
