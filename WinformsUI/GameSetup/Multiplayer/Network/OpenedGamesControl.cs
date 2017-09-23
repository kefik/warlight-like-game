namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects.Message;

    public partial class OpenedGamesControl : UserControl
    {
        private IEnumerable<OpenedGameHeaderMessageObject> gameHeaders;

        public OpenedGamesControl()
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

            gameHeaders = await user.GetListOfOpenedGamesAsync() ?? new List<OpenedGameHeaderMessageObject>();

            foreach (OpenedGameHeaderMessageObject gameHeader in gameHeaders)
            {
                Invoke(new Action(() => multiDayListBox.Items.Add(gameHeader)));
            }
        }

        private async void OpenGame(object sender, EventArgs e)
        {
            if (gameHeaders == null)
            {
                return;
            }

            int selectedIndex = multiDayListBox.SelectedIndex;

            if (selectedIndex == -1)
            {
                return;
            }

            OpenedGameHeaderMessageObject chosenGame = gameHeaders.ElementAtOrDefault(selectedIndex);

            if (chosenGame == null)
            {
                return;
            }

            MyNetworkUser networkUser = Global.MyUser as MyNetworkUser;
            if (networkUser == null)
            {
                return;
            }

            JoinNetworkGameForm joinForm = new JoinNetworkGameForm();
            DialogResult dialogResult = joinForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                HumanPlayer player = joinForm.GetPlayer();

                await networkUser.JoinOpenedGameAsync(player, chosenGame.GameId);
            }
        }
    }
}
