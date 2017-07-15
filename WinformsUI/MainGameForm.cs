using System;
using System.Drawing;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.Game;
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

        public MainGameForm()
        {
            InitializeComponent();
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
                    // removes previous
                    singleplayerGameOptionsControl?.Dispose();
                    singleplayerGameOptionsControl = null;
                    // loads game screens
                    inGame = new InGameControl()
                    {
                        Parent = singleplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                    break;
                case GameType.MultiplayerHotseat:
                    // removes previous
                    hotseatGameOptionsControl?.Dispose();
                    hotseatGameOptionsControl = null;
                    // loads game screens
                    inGame = new InGameControl()
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                    break;
                case GameType.MultiplayerNetwork:
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
                Dock = DockStyle.Fill
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
                hotseatGameOptionsControl = new HotseatGameOptionsControl()
                {
                    Parent = multiplayerTabPage,
                    Dock = DockStyle.Fill
                };
                hotseatGameOptionsControl.Show();
            }

            void LoadNetworkControls()
            {
                networkGameOptionsControl?.Dispose();
                networkGameOptionsControl = new NetworkGameOptionsControl()
                {
                    Parent = multiplayerTabPage,
                    Dock = DockStyle.Fill
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
                            LoadNetworkControls();
                            break;
                        default:
                            typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                            return;
                    }
                    break;
                case DialogResult.Cancel:
                    // TODO: invokes select index event, I want it to return back to previous index without deleting it
                    typeGameChoiceTabControl.SelectedIndex = previousTabSelectedIndex;
                    break;
                default:
                    return;
            }
        }
        
        void LoadSettingsControls()
        {
            throw new NotImplementedException();
        }
        private void TabChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.GetType().ToString());
            // TODO: warnings about leaving game unsaved
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
