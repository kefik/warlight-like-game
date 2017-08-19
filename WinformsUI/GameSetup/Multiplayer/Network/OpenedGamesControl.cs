namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.NetworkCommObjects.Message;

    public partial class OpenedGamesControl : UserControl
    {
        IEnumerable<OpenedGameHeaderMessageObject> gameHeaders;

        public OpenedGamesControl()
        {
            InitializeComponent();
        }

        void ControlLoad(object sender, EventArgs e)
        {
            LoadTableAsync();
        }

        async void LoadTableAsync()
        {
            MyNetworkUser user = Global.MyUser as MyNetworkUser;

            if (user == null) throw new ArgumentException();

            gameHeaders = await user.GetListOfOpenedGamesAsync() ?? new List<OpenedGameHeaderMessageObject>();

            foreach (var gameHeader in gameHeaders) Invoke(new Action(() => multiDayListBox.Items.Add(gameHeader)));
        }

        private async void OpenGame(object sender, EventArgs e)
        {
            if (gameHeaders == null) return;

            int selectedIndex = multiDayListBox.SelectedIndex;

            if (selectedIndex == -1) return;

            var chosenGame = gameHeaders.ElementAtOrDefault(selectedIndex);

            if (chosenGame == null) return;

            var networkUser = Global.MyUser as MyNetworkUser;
            if (networkUser == null) return;

            var joinForm = new JoinNetworkGameForm();
            var dialogResult = joinForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                var player = joinForm.GetPlayer();

                await networkUser.JoinOpenedGameAsync(player, chosenGame.GameId);
            }
        }
    }
}
