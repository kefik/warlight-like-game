using System;
using System.Windows.Forms;
using GameObjectsLib.Game;
using GameObjectsLib.GameUser;
using WinformsUI.GameSetup.Multiplayer;
using WinformsUI.GameSetup.Multiplayer.Hotseat;
using WinformsUI.GameSetup.Multiplayer.Network;
using WinformsUI.GameSetup.Singleplayer;
using WinformsUI.InGame;

namespace WinformsUI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameObjectsLib;

    public partial class MainGameForm : Form
    {
        int previousTabSelectedIndex;

        SingleplayerGameOptionsControl singleplayerGameOptionsControl;
        HotseatGameOptionsControl hotseatGameOptionsControl;
        NetworkGameOptionsControl networkGameOptionsControl;

        InGameControl inGame;

        User myUser = new LocalUser("Me");

        User MyUser
        {
            get { return myUser; }
            set
            {
                myUser = value;
                UserChanged(value);
            }
        }

        public MainGameForm()
        {
            InitializeComponent();

            // initialize default tab
            previousTabSelectedIndex = typeGameChoiceTabControl.SelectedIndex = 0;
            LoadSingleplayerControls();
        }

        /// <summary>
        /// Loads proper screens starting newly created game the game.
        /// </summary>
        /// <param name="game">Instance representing the game to be started.</param>
        private void LoadInGameScreen(Game game)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    // remove previous
                    singleplayerGameOptionsControl?.Dispose();
                    singleplayerGameOptionsControl = null;
                    // load game screen
                    inGame = new InGameControl
                    {
                        Parent = singleplayerTabPage,
                        Dock = DockStyle.Fill,
                        Game = game
                    };
                    break;
                case GameType.MultiplayerHotseat:
                    // remove previous
                    hotseatGameOptionsControl?.Dispose();
                    hotseatGameOptionsControl = null;
                    // load game screen
                    inGame = new InGameControl
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill,
                        Game = game
                    };
                    break;
                case GameType.MultiplayerNetwork:
                    // removes previous
                    networkGameOptionsControl?.Dispose();
                    networkGameOptionsControl = null;
                    // loads game screens
                    inGame = new InGameControl
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill,
                        Game = game
                    };
                    break;
            }

            inGame.Show();
        }

        private void LoadGame(Game game)
        {
            LoadInGameScreen(game);
        }



        private void StartNewGame(Game game)
        {
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    // save the game
                    using (var db = new UtilsDbContext())
                    {
                        game.Save(db);
                    }
                    break;
                case GameType.MultiplayerHotseat:
                    // save the game
                    using (var db = new UtilsDbContext())
                    {
                        game.Save(db);
                    }
                    break;
                case GameType.MultiplayerNetwork:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            LoadInGameScreen(game);
        }

        private async Task CreateNewGame(HumanPlayer creatingPlayer, ICollection<AiPlayer> aiPlayers, string mapName, int freeSlotsCount)
        {
            if (TryToLogIn())
            {
                var networkUser = (MyNetworkUser)myUser;
                bool successful = await networkUser.CreateGameAsync(creatingPlayer, aiPlayers, mapName, freeSlotsCount);
                if (successful == false)
                {
                    MessageBox.Show($"The game could not be created.");
                    return;
                }
                
                // TODO: load appropriate screen
                typeGameChoiceTabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Resets previously loaded singleplayer control and loads new, resetted one.
        /// </summary>
        void LoadSingleplayerControls()
        {
            singleplayerGameOptionsControl?.Dispose();

            singleplayerGameOptionsControl = new SingleplayerGameOptionsControl
            {
                Parent = singleplayerTabPage,
                Dock = DockStyle.Fill,
                GetUser = () => MyUser,
                SetUser = user => MyUser = user
            };
            singleplayerGameOptionsControl.OnNewGameStarted += StartNewGame;
            singleplayerGameOptionsControl.OnGameLoaded += LoadGame;
            singleplayerGameOptionsControl.Show();
        }

        /// <summary>
        /// Resets previously loaded multiplayer control and loads new, resetted one.
        /// </summary>
        void LoadMultiplayerControls()
        {
            void LoadHotseatControls()
            {
                hotseatGameOptionsControl?.Dispose();
                networkGameOptionsControl?.Dispose();
                networkGameOptionsControl = null;
                hotseatGameOptionsControl = new HotseatGameOptionsControl
                {
                    Parent = multiplayerTabPage,
                    Dock = DockStyle.Fill,
                    GetUser = () => MyUser,
                    SetUser = user => MyUser = user
                };
                hotseatGameOptionsControl.Show();

                hotseatGameOptionsControl.OnGameStarted += StartNewGame;
                hotseatGameOptionsControl.OnGameLoaded += LoadGame;
            }

            void LoadNetworkControls()
            {
                networkGameOptionsControl?.Dispose();
                hotseatGameOptionsControl?.Dispose();
                hotseatGameOptionsControl = null;
                networkGameOptionsControl = new NetworkGameOptionsControl
                {
                    Parent = multiplayerTabPage,
                    Dock = DockStyle.Fill,
                    GetUser = () => MyUser,
                    SetUser = user => MyUser = user
                };
                networkGameOptionsControl.OnGameCreated += CreateNewGame;
                networkGameOptionsControl.Show();

            }

            var gameTypeChoiceForm = new GameTypeChoiceForm();
            var dialogResult = gameTypeChoiceForm.ShowDialog();

            switch (dialogResult)
            {
                case DialogResult.OK:
                    switch (gameTypeChoiceForm.MultiplayerGameType)
                    {
                        case GameType.MultiplayerHotseat:
                            LoadHotseatControls();
                            break;
                        case GameType.MultiplayerNetwork:
                            // network => find out if user is logged
                            if (TryToLogIn())
                            {
                                LoadNetworkControls();
                            }
                            else
                            {
                                typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                            }
                            break;
                        default:
                            // return back to previous stage
                            typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                            return;
                    }
                    break;
                case DialogResult.Cancel:
                default:
                    // TODO: invokes select index event, I want it to return back to previous index without deleting it
                    typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                    return;
            }
        }

        public void UserChanged(User newUser)
        {
            // TODO: user changed broadcast to others
            if (MyUser.UserType == UserType.MyNetworkUser)
                loggedInLabel.Text = $"You are currently logged in as {MyUser.Name}.";
            else
            {
                loggedInLabel.Text = $"You are currently logged in as a local user.";
            }
        }

        bool TryToLogIn()
        {
            if (MyUser.UserType != UserType.MyNetworkUser)
            {
                ServerLoggingForm serverLoggingForm = new ServerLoggingForm();
                var dialogResult = serverLoggingForm.ShowDialog();
                switch (dialogResult)
                {
                    case DialogResult.OK:
                        MyUser = serverLoggingForm.User;
                        break;
                    default:
                        typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                        return false;
                }
            }
            else
            {
                var converted = (MyNetworkUser)MyUser;
                bool amILogged = converted.IsLoggedIn();

                if (!amILogged)
                {
                    ServerLoggingForm serverLoggingForm = new ServerLoggingForm();
                    var dialogResult = serverLoggingForm.ShowDialog();
                    switch (dialogResult)
                    {
                        case DialogResult.OK:
                            MyUser = serverLoggingForm.User;
                            break;
                        default:
                            typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                            return false;
                    }
                }
            }
            return true;
        }
        void LoadSettingsControls()
        {
            if (TryToLogIn())
            {
                var newUser = (MyNetworkUser)MyUser;
                playerNameTextBox.Text = newUser.Name;
                emailTextBox.Text = newUser.Email;
                Refresh();
            }
            else
            {
                typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
            }
        }
        private void TabChanged(object sender, EventArgs e)
        {
            // TODO: warnings about not saving the changes

            inGame?.Dispose();
            inGame = null;

            switch (typeGameChoiceTabControl.SelectedIndex)
            {
                case 0: // singleplayer
                    LoadSingleplayerControls();
                    break;
                case 1: // multiplayer
                    LoadMultiplayerControls();
                    break;
                case 2: // settings
                    LoadSettingsControls();
                    break;
                default:
                    return;

            }
            previousTabSelectedIndex = typeGameChoiceTabControl.SelectedIndex;
        }

    }
}
