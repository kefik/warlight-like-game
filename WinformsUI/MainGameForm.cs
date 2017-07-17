using System;
using System.Drawing;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.Game;
using ConquestObjectsLib.GameUser;
using WinformsUI.Game;
using WinformsUI.GameSetup.Multiplayer;
using WinformsUI.GameSetup.Multiplayer.Hotseat;
using WinformsUI.GameSetup.Multiplayer.Network;
using WinformsUI.GameSetup.Singleplayer;

namespace WinformsUI
{
    public partial class MainGameForm : Form
    {
        int previousTabSelectedIndex;
        
        SingleplayerGameOptionsControl singleplayerGameOptionsControl;
        HotseatGameOptionsControl hotseatGameOptionsControl;
        NetworkGameOptionsControl networkGameOptionsControl;

        InGameControl inGame;

        User myUser = new LocalUser("Me");

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
        private void StartGame(ConquestObjectsLib.Game.Game game)
        {
            // start the game
            game.Start();
            
            switch (game.GameType)
            {
                case GameType.SinglePlayer:
                    // remove previou
                    singleplayerGameOptionsControl?.Dispose();
                    singleplayerGameOptionsControl = null;
                    // load game screen
                    inGame = new InGameControl()
                    {
                        Parent = singleplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                    break;
                case GameType.MultiplayerHotseat:
                    // remove previou
                    hotseatGameOptionsControl?.Dispose();
                    hotseatGameOptionsControl = null;
                    // load game screen
                    inGame = new InGameControl()
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                    break;
                case GameType.MultiplayerNetwork:
                    // TODO: fix
                    // removes previous
                    networkGameOptionsControl?.Dispose();
                    networkGameOptionsControl = null;
                    // loads game screens
                    inGame = new InGameControl()
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                    break;
            }
            
            inGame.Show();
            // TODO: fix starting phase
        }


        /// <summary>
        /// Resets previously loaded singleplayer control and loads new, resetted one.
        /// </summary>
        void LoadSingleplayerControls()
        {
            singleplayerGameOptionsControl?.Dispose();

            singleplayerGameOptionsControl = new SingleplayerGameOptionsControl()
            {
                Parent = this.singleplayerTabPage,
                Dock = DockStyle.Fill,
                User = myUser
            };
            singleplayerGameOptionsControl.OnGameStarted += StartGame;
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
                hotseatGameOptionsControl = new HotseatGameOptionsControl()
                {
                    Parent = multiplayerTabPage,
                    Dock = DockStyle.Fill,
                    MyUser = myUser
                };
                hotseatGameOptionsControl.Show();

                hotseatGameOptionsControl.OnGameStarted += StartGame;
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
                    MyUser = myUser
                };
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
                            if (myUser?.GetType() == typeof(MyNetworkUser)) // user is already logged
                            {
                                LoadNetworkControls();
                                return;
                            }
                            // user is not logged => log him
                            var serverLoggingForm = new ServerLoggingForm();
                            var dialogLogging = serverLoggingForm.ShowDialog();

                            switch (dialogLogging)
                            {
                                case DialogResult.OK:
                                    UserChanged(serverLoggingForm.User);
                                    LoadNetworkControls();
                                    break;
                                case DialogResult.Cancel:
                                default:
                                    // return back to previous stage
                                    typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                                    return;
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
            myUser = newUser;
        }
        
        void LoadSettingsControls()
        {
            throw new NotImplementedException();
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
