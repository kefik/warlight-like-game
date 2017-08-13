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
    public partial class MyHumanPlayerControl : UserControl
    {
        public MyHumanPlayerControl()
        {
            InitializeComponent();

            // initialize
            playerNameTextBox.Text = User.Name;
            playerNameTextBox.Enabled = true;
            PlayerColor = KnownColor.Green;
        }
        
        /// <summary>
        /// Represents clients user. Cannot be null.
        /// </summary>
        public User User
        {
            get
            {
                if (GetUser == null) return new LocalUser("Me");
                return GetUser();
            }
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
                playerNameTextBox.Text = value.Name;
                SetUser?.Invoke(value);
            }
        }
        public Func<User> GetUser;
        public Action<User> SetUser;


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
            get { return User.Name; }
            private set
            {
                User.Name = value;
                playerNameTextBox.Text = value;
            }
        }


        /// <summary>
        /// Creates and returns new instance of player represented by this control.
        /// </summary>
        /// <returns>New instance of the player represented by this control.</returns>
        public HumanPlayer GetPlayer()
        {
            return new HumanPlayer(User, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int) PlayerColor >= 173) PlayerColor = (KnownColor) 0;
                    else PlayerColor++;
                    break;
                case MouseButtons.Right:
                    if ((int)PlayerColor <= 0) PlayerColor = (KnownColor)173;
                    else PlayerColor--;
                    break;
            }
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            switch (User.UserType)
            {
                case UserType.LocalUser:
                    User.Name = Name;
                    break;
                case UserType.NetworkUser:
                    playerNameTextBox.Text = PlayerName;
                    break;
            }
        }
    }
}
