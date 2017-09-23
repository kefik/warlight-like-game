namespace WinformsUI.HelperControls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.GameUser;

    public partial class MyHumanPlayerControl : UserControl, IDisposable
    {
        public MyHumanPlayerControl()
        {
            InitializeComponent();

            Global.OnUserChanged += UserChanged;
        }

        private KnownColor playerColor;

        /// <summary>
        ///     Represents clients color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return playerColor; }
            private set
            {
                playerColor = value;
                colorButton.BackColor = Color.FromKnownColor(PlayerColor);
            }
        }

        /// <summary>
        ///     Represents name of player this control is representing.
        /// </summary>
        public string PlayerName
        {
            get { return Global.MyUser.Name; }
            private set
            {
                Global.MyUser = new LocalUser(value);
                playerNameTextBox.Text = value;
            }
        }


        /// <summary>
        ///     Creates and returns new instance of player represented by this control.
        /// </summary>
        /// <returns>New instance of the player represented by this control.</returns>
        public HumanPlayer GetPlayer()
        {
            return new HumanPlayer(Global.MyUser, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int) PlayerColor >= 173)
                    {
                        PlayerColor = 0;
                    }
                    else
                    {
                        PlayerColor++;
                    }
                    break;
                case MouseButtons.Right:
                    if ((int) PlayerColor <= 0)
                    {
                        PlayerColor = (KnownColor) 173;
                    }
                    else
                    {
                        PlayerColor--;
                    }
                    break;
            }
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            switch (Global.MyUser.UserType)
            {
                case UserType.LocalUser:
                    Global.MyUser = new LocalUser(playerNameTextBox.Text);
                    break;
            }
        }

        private void ControlLoad(object sender, EventArgs e)
        {
            playerNameTextBox.Text = Global.MyUser.Name;

            UserChanged(Global.MyUser);

            PlayerColor = KnownColor.Green;
        }

        public new void Dispose()
        {
            Global.OnUserChanged -= UserChanged;
        }

        private void UserChanged(User user)
        {
            Invoke(user.UserType == UserType.MyNetworkUser
                ? (() => playerNameTextBox.Enabled = false)
                : new Action(() => playerNameTextBox.Enabled = true));
        }
    }
}
