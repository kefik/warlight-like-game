#define TIME_MEASURE
#if DEBUG
#define TRACE_CONSOLE
#endif

namespace WinformsUI.InGame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Client.Entities;
    using GameHandlersLib;
    using GameHandlersLib.GameHandlers;
    using GameHandlersLib.MapHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;
    using HelperObjects;

    public partial class SimulatorInGameControl : UserControl
    {
        private SimulationFlowHandler simulationFlowHandler;

        private string logFileName;

        public SimulatorInGameControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer,
                value: true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void InitializePlayersPerspectiveComboBox(
            IEnumerable<Player> players)
        {
            var list = new List<ComboBoxItem>
            {
                // prepend empty value
                new ComboBoxItem {Value = null, Text = "GOD"}
            };
            foreach (Player player in players)
            {
                list.Add(new ComboBoxItem
                {
                    Text = player.Name,
                    Value = player.Id
                });
            }

            playerPerspectiveComboBox.ValueMember = "Value";
            playerPerspectiveComboBox.DisplayMember = "Text";
            playerPerspectiveComboBox.DataSource = list;
        }

        public void Initialize(Game game)
        {
            if (game == null)
            {
                throw new ArgumentException("Game cannot be null.");
            }

            MapImageProcessor mapImageProcessor;
            using (UtilsDbContext db = new UtilsDbContext())
            {
                MapInfo mapInfo =
                (from item in db.Maps
                 where item.Id == game.Map.Id
                 select item).First();

                mapImageProcessor = MapImageProcessor.Create(game.Map,
                    mapInfo.ImageColoredRegionsPath,
                    mapInfo.ColorRegionsTemplatePath,
                    mapInfo.ImagePath, game.IsFogOfWar);
            }

            simulationFlowHandler = new SimulationFlowHandler(game,
                mapImageProcessor, playerPerspective: null);

            gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            gameMapPictureBox.Image = simulationFlowHandler
                .ImageProcessor.MapImage;
            gameMapPictureBox.Height = gameMapPictureBox.Image.Height;
            gameMapPictureBox.Width = gameMapPictureBox.Image.Width;
            gameMapPictureBox.BackgroundImage = simulationFlowHandler
                .ImageProcessor.TemplateImage;

            // resize window
            //ParentForm.Size =
            //    new Size(gameMapPictureBox.Location.X + gameMapPictureBox.Width, Math.Max(gameStateMenuPanel.Height, gameMapPictureBox.Height));
            // TODO: dynamically
            ParentForm.Width = 1107;
            ParentForm.Height = 541;

            if (game.IsFogOfWar)
            {
                InitializePlayersPerspectiveComboBox(game.Players);
                playerPerspectiveComboBox.DrawMode =
                    DrawMode.OwnerDrawVariable;
            }
            else
            {
                playerPerspectiveLabel.Hide();
                playerPerspectiveComboBox.Hide();
            }

            simulationFlowHandler.OnImageChanged +=
                gameMapPictureBox.Refresh;
            simulationFlowHandler.OnImageChanged +=
                RefreshRoundNumber;

#if DEBUG
            logFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
#endif
        }

        public async void PlayOrStopButtonClick(object sender,
            EventArgs args)
        {
            TimeSpan timeForBotMove = new TimeSpan(days: 0, hours: 0,
                minutes: 0, seconds: 0,
                milliseconds: (int) botThinkingTimeNumericUpDown
                    .Value);
            if (!simulationFlowHandler.IsRunning)
            {
#if DEBUG
                // add trace listener
                Debug.Listeners.Add(new TextWriterTraceListener(
                    new StreamWriter(logFileName, append: true)));
#endif
                try
                {
                    await simulationFlowHandler
                        .StartOrContinueEvaluationAsync(
                            timeForBotMove);
                    playPauseButton.Enabled = true;
                }
                catch (GameFinishedException)
                {
                    // ignore for now
                    playPauseButton.Enabled = false;
                }
#if DEBUG
                finally
                {
                    // close the file listener
                    TraceListener lastListener =
                        Debug.Listeners[Debug.Listeners.Count - 1];
                    lastListener.Close();
                    Debug.Listeners.RemoveAt(
                        Debug.Listeners.Count - 1);
                }
#endif
            }
            else
            {
                playPauseButton.Enabled = false;
                simulationFlowHandler.PauseEvaluation();
            }
        }

        public void NextActionButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToNextAction();
            }
        }

        public void PreviousActionButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToPreviousAction();
            }
        }

        public void NextRoundButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToNextRound();
            }
        }

        public void PreviousRoundButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToPreviousRound();
            }
        }

        public void EndOfTheGameButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToEnd();
            }
        }

        public void BeginningOfTheGameButtonClick(object sender,
            EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToBeginning();
            }
        }

        private void PlayerPerspectiveDrawItem(object sender,
            DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                ComboBoxItem comboBoxItem =
                    (ComboBoxItem) ((ComboBox) sender).Items[e.Index];
                Player selectedPlayer =
                    simulationFlowHandler.Game.Players.FirstOrDefault(
                        x => x.Id == comboBoxItem.Value);

                Color playerColor;
                playerColor = selectedPlayer == null
                    ? Color.White
                    : Color.FromKnownColor(selectedPlayer.Color);

                Font f = new Font("Arial", emSize: 9,
                    style: FontStyle.Regular);
                Color c = playerColor;
                Brush b = new SolidBrush(c);
                g.FillRectangle(b, rect.X, rect.Y, rect.Width,
                    rect.Height);

                g.DrawString(comboBoxItem.Text, f, Brushes.Black,
                    rect.X, rect.Top);
            }
        }

        private void PlayerPerspectiveChanged(object sender,
            EventArgs e)
        {
            ComboBox typedSender = (ComboBox) sender;
            int selectedIndex = typedSender.SelectedIndex;

            ComboBoxItem comboBoxItem =
                typedSender.Items[selectedIndex] as ComboBoxItem;
            if (comboBoxItem == null)
            {
                return;
            }

            Player player =
                simulationFlowHandler.Game.Players.FirstOrDefault(
                    x => x.Id == comboBoxItem.Value);

            simulationFlowHandler.ChangePlayerPerspective(player);
        }

        private void RefreshRoundNumber()
        {
            int displayedRoundNumber = simulationFlowHandler
                .GetDisplayedRoundNumber();
            roundNumber.Text = displayedRoundNumber.ToString();
        }

        private void displayedRoundLabel_Click(object sender, EventArgs e)
        {
            // TODO: remove
            MessageBox.Show($"Width: {ParentForm.Width}, Height: {ParentForm.Height}");
        }
    }
}
