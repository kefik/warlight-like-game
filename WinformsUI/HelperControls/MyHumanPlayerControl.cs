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
        public KnownColor Color
        {
            get { return player.Color; }
            set
            {
                player = new HumanPlayer(User, value);
                colorButton.BackColor = System.Drawing.Color.FromKnownColor(Color);
            }
        }
        /// <summary>
        /// Represents players name.
        /// </summary>
        public string Name
        {
            get { return player.Name; }
        }
        

        public MyHumanPlayerControl()
        {
            InitializeComponent();
            
            player = new HumanPlayer(new LocalUser("Me"), KnownColor.Red);
            UserChanged(User);
        }
        
        void UserChanged(User user)
        {
            player = new HumanPlayer(user, Color);
            playerNameTextBox.Text = user.Name;
            switch (user.UserType)
            {
                case UserType.Local:
                    playerNameTextBox.Enabled = true;
                    break;
                case UserType.Network:
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
            return new HumanPlayer(User, Color);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Color++; // TODO: fix so it doesnt overflow or underflow
                    break;
                case MouseButtons.Right:
                    Color--;
                    break;
            }
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            if (User.GetType() == typeof(MyNetworkUser))
            {
                playerNameTextBox.Text = Name;
            }
            else
            {
                player = new HumanPlayer(new LocalUser(playerNameTextBox.Text), Color);
            }
        }
    }
}
