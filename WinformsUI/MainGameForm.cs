#define DEBUG_CONSOLE

#if !DEBUG
#undef DEBUG_CONSOLE
#endif

namespace WinformsUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;
    using GameSetup.Multiplayer;
    using GameSetup.Multiplayer.Hotseat;
    using GameSetup.Multiplayer.Network;
    using GameSetup.Simulator;
    using GameSetup.Singleplayer;
    using InGame;

    public partial class MainGameForm : Form
    {
        private int previousTabSelectedIndex;

        private SingleplayerGameOptionsControl
            singleplayerGameOptionsControl;

        private HotseatGameOptionsControl hotseatGameOptionsControl;
        private NetworkGameOptionsControl networkGameOptionsControl;

        private SimulatorGameOptionsControl
            simulatorGameOptionsControl;

        private InGameControl inGame;
        private SimulatorInGameControl simulatorInGame;

        public MainGameForm()
        {
            InitializeComponent();

            // initialize default tab
            previousTabSelectedIndex =
                typeGameChoiceTabControl.SelectedIndex = 0;
            LoadSingleplayerControls();
        }

        /// <summary>
        ///     Loads proper screens starting newly created game the game.
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
                        Dock = DockStyle.Fill
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
                        Dock = DockStyle.Fill
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
                        Dock = DockStyle.Fill
                    };
                    break;
            }
            inGame.Initialize(game);
            inGame.Show();
        }

        private void LoadSimulatorInGameScreen(Game game)
        {
            // remove previous
            simulatorGameOptionsControl?.Dispose();
            simulatorGameOptionsControl = null;
            // load game screen
            simulatorInGame = new SimulatorInGameControl
            {
                Parent = simulatorTabPage,
                Dock = DockStyle.Fill
            };
            simulatorInGame.Initialize(game);
            simulatorInGame.Show();
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
                    using (UtilsDbContext db = new UtilsDbContext())
                    {
                        game.Save(db);
                    }
                    break;
                case GameType.MultiplayerHotseat:
                    // save the game
                    using (UtilsDbContext db = new UtilsDbContext())
                    {
                        game.Save(db);
                    }
                    break;
                case GameType.MultiplayerNetwork: break;
                default: throw new ArgumentOutOfRangeException();
            }
            LoadInGameScreen(game);
        }

        private async Task CreateNewGame(HumanPlayer creatingPlayer,
            ICollection<AiPlayer> aiPlayers, string mapName,
            int freeSlotsCount)
        {
            if (TryToLogIn())
            {
                MyNetworkUser networkUser =
                    (MyNetworkUser) Global.MyUser;
                bool successful =
                    await networkUser.CreateGameAsync(creatingPlayer,
                        aiPlayers, mapName, freeSlotsCount);
                if (successful == false)
                {
                    Invoke(new Action(() => MessageBox.Show(
                        $"The game could not be created.")));
                    return;
                }

                // TODO: load appropriate screen
                Invoke(new Action(() => typeGameChoiceTabControl
                    .SelectedIndex = 0));
            }
        }

        /// <summary>
        ///     Resets previously loaded singleplayer control and loads new, resetted one.
        /// </summary>
        private void LoadSingleplayerControls()
        {
            singleplayerGameOptionsControl?.Dispose();

            singleplayerGameOptionsControl =
                new SingleplayerGameOptionsControl
                {
                    Parent = singleplayerTabPage,
                    Dock = DockStyle.Fill
                };
            singleplayerGameOptionsControl.OnNewGameStarted +=
                StartNewGame;
            singleplayerGameOptionsControl.OnGameLoaded += LoadGame;
            singleplayerGameOptionsControl.Show();
        }

        /// <summary>
        ///     Resets previously loaded multiplayer control and loads new, resetted one.
        /// </summary>
        private void LoadMultiplayerControls()
        {
            void LoadHotseatControls()
            {
                hotseatGameOptionsControl?.Dispose();
                networkGameOptionsControl?.Dispose();
                networkGameOptionsControl = null;
                hotseatGameOptionsControl =
                    new HotseatGameOptionsControl
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                hotseatGameOptionsControl.Show();

                hotseatGameOptionsControl.OnGameStarted +=
                    StartNewGame;
                hotseatGameOptionsControl.OnGameLoaded += LoadGame;
            }

            void LoadNetworkControls()
            {
                networkGameOptionsControl?.Dispose();
                hotseatGameOptionsControl?.Dispose();
                hotseatGameOptionsControl = null;
                networkGameOptionsControl =
                    new NetworkGameOptionsControl
                    {
                        Parent = multiplayerTabPage,
                        Dock = DockStyle.Fill
                    };
                networkGameOptionsControl.OnGameCreated +=
                    CreateNewGame;
                networkGameOptionsControl.Show();
            }

            GameTypeChoiceForm gameTypeChoiceForm =
                new GameTypeChoiceForm();
            DialogResult dialogResult =
                gameTypeChoiceForm.ShowDialog();

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
                                typeGameChoiceTabControl
                                        .SelectedIndex =
                                    previousTabSelectedIndex;
                            }
                            break;
                        default:
                            // return back to previous stage
                            typeGameChoiceTabControl.SelectedIndex =
                                previousTabSelectedIndex;
                            return;
                    }
                    break;
                case DialogResult.Cancel:
                default:
                    // TODO: invokes select index event, I want it to return back to previous index without deleting it
                    typeGameChoiceTabControl.SelectedIndex =
                        previousTabSelectedIndex;
                    return;
            }
        }

        private void LoadSimulatorControls()
        {
            simulatorGameOptionsControl?.Dispose();

            simulatorGameOptionsControl =
                new SimulatorGameOptionsControl
                {
                    Parent = simulatorTabPage,
                    Dock = DockStyle.Fill
                };

            simulatorGameOptionsControl.OnSimulationStarted +=
                LoadSimulatorInGameScreen;
            simulatorGameOptionsControl.Show();
        }

        public void UserChanged(User newUser)
        {
            if (Global.MyUser.UserType == UserType.MyNetworkUser)
            {
                loggedInLabel.Text =
                    $"You are currently logged in as {Global.MyUser.Name}.";
            }
            else
            {
                loggedInLabel.Text =
                    $"You are currently logged in as a local user.";
            }
        }

        private bool TryToLogIn()
        {
            if (Global.MyUser.UserType != UserType.MyNetworkUser)
            {
                ServerLoggingForm serverLoggingForm =
                    new ServerLoggingForm();

                DialogResult dialogResult;
                if (InvokeRequired)
                {
                    dialogResult =
                        (DialogResult) Invoke(new Action(
                            () => serverLoggingForm.ShowDialog()));
                }
                else
                {
                    dialogResult = serverLoggingForm.ShowDialog();
                }

                switch (dialogResult)
                {
                    case DialogResult.OK:
                        Global.MyUser = serverLoggingForm.User;
                        break;
                    default:
                        if (InvokeRequired)
                        {
                            Invoke(
                                new Action(
                                    () => typeGameChoiceTabControl
                                            .SelectedIndex =
                                        previousTabSelectedIndex));
                        }
                        else
                        {
                            typeGameChoiceTabControl.SelectedIndex =
                                previousTabSelectedIndex;
                        }
                        return false;
                }
            }
            else
            {
                MyNetworkUser converted =
                    (MyNetworkUser) Global.MyUser;
                bool amILogged = converted.IsLoggedIn();

                if (!amILogged)
                {
                    ServerLoggingForm serverLoggingForm =
                        new ServerLoggingForm();

                    DialogResult dialogResult;
                    if (InvokeRequired)
                    {
                        dialogResult =
                            (DialogResult) Invoke(new Action(
                                () => serverLoggingForm
                                    .ShowDialog()));
                    }
                    else
                    {
                        dialogResult = serverLoggingForm.ShowDialog();
                    }

                    switch (dialogResult)
                    {
                        case DialogResult.OK:
                            Global.MyUser = serverLoggingForm.User;
                            break;
                        default:
                            if (InvokeRequired)
                            {
                                Invoke(new Action(
                                    () => typeGameChoiceTabControl
                                            .SelectedIndex =
                                        previousTabSelectedIndex));
                            }
                            else
                            {
                                typeGameChoiceTabControl
                                        .SelectedIndex =
                                    previousTabSelectedIndex;
                            }
                            return false;
                    }
                }
            }
            return true;
        }

        private void LoadSettingsControls()
        {
            if (TryToLogIn())
            {
                MyNetworkUser newUser = (MyNetworkUser) Global.MyUser;
                playerNameTextBox.Text = newUser.Name;
                emailTextBox.Text = newUser.Email;
                Refresh();
            }
            else
            {
                typeGameChoiceTabControl.SelectedIndex =
                    previousTabSelectedIndex;
            }
        }

        private void TabChanged(object sender, EventArgs e)
        {
            // TODO: warnings about not saving the changes

            inGame?.Dispose();
            inGame = null;

            simulatorInGame?.Dispose();
            simulatorInGame = null;

            Global.PlayerColorPicker.Reset();

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
                case 3: // simulator
                    LoadSimulatorControls();
                    break;
                default: return;
            }
            previousTabSelectedIndex =
                typeGameChoiceTabControl.SelectedIndex;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            Global.OnUserChanged += UserChanged;

#if DEBUG_CONSOLE
            if (!Debugger.IsAttached)
            {
                AllocConsole();
                Debug.Listeners.Add(new ConsoleTraceListener());
            }
#endif
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {
            Global.OnUserChanged -= UserChanged;
        }

#if DEBUG_CONSOLE

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
#endif
    }
}
