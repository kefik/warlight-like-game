namespace WinformsUI.HelperControls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;
    using Helpers;

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
            set
            {
                playerColor = value;
                colorButton.BackColor = Color.FromKnownColor(value);
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
                {
                    var newColor =
                        Global.PlayerColorPicker.PickNext(PlayerColor);

                    if (newColor != null)
                    {
                        Global.PlayerColorPicker.ReturnColor(PlayerColor);
                        PlayerColor = newColor.Value;
                    }
                    break;
                }
                case MouseButtons.Right:
                {
                    var newColor =
                        Global.PlayerColorPicker.PickPrevious(PlayerColor);

                    if (newColor != null)
                    {
                        Global.PlayerColorPicker.ReturnColor(PlayerColor);
                        PlayerColor = newColor.Value;
                    }
                    break;
                }
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
        }

        protected override void DestroyHandle()
        {
            Global.OnUserChanged -= UserChanged;
            base.DestroyHandle();
        }

        private void UserChanged(User user)
        {
            if (playerNameTextBox?.IsDisposed == true)
            {
                return;
            }
            this.InvokeIfRequired(user.UserType == UserType.MyNetworkUser
                ? new Action(() => playerNameTextBox.Enabled = false)
                : (() => playerNameTextBox.Enabled = true));
        }
    }
}
