namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects.Message;
    using Helpers;

    public partial class MyGamesControl : UserControl
    {
        private IEnumerable<GameHeaderMessageObject> gameHeaders;

        public MyGamesControl()
        {
            InitializeComponent();
        }

        private void ControlLoad(object sender, EventArgs e)
        {
            LoadTableAsync();
        }

        private async void LoadTableAsync()
        {
            MyNetworkUser user = Global.MyUser as MyNetworkUser;

            if (user == null)
            {
                throw new ArgumentException();
            }

            gameHeaders = await user.GetListOfMyGamesAsync() ?? new List<GameHeaderMessageObject>();

            foreach (GameHeaderMessageObject gameHeader in gameHeaders)
            {
                if (gameHeader.GetType() == typeof(OpenedGameHeaderMessageObject))
                {
                    multiDayListBox.InvokeIfRequired(() => multiDayListBox.Items.Add(gameHeader));
                }
                else if (gameHeader.GetType() == typeof(StartedGameHeaderMessageObject))
                {
                    multiDayListBox.InvokeIfRequired(() => multiDayListBox.Items.Add(gameHeader));
                }
            }
        }

        private void OpenButtonClick(object sender, EventArgs e)
        {
        }
    }
}
