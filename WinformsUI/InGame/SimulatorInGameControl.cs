using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.InGame
{
    using Client.Entities;
    using Common.Extensions;
    using GameAi.Data;
    using GameHandlersLib.GameHandlers;
    using GameHandlersLib.MapHandlers;
    using GameObjectsLib.Game;
    using GameObjectsLib.Players;
    using HelperObjects;

    public partial class SimulatorInGameControl : UserControl
    {
        private SimulationFlowHandler simulationFlowHandler;

        public SimulatorInGameControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void InitializePlayersPerspectiveComboBox(IEnumerable<Player> players)
        {
            var list = new List<ComboBoxItem>()
            {
                // prepend empty value
                new ComboBoxItem()
                {
                    Value = null,
                    Text = "GOD"
                }
            };
            foreach (var player in players)
            {
                list.Add(new ComboBoxItem()
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
                MapInfo mapInfo = (from item in db.Maps
                                   where item.Id == game.Map.Id
                                   select item).First();

                mapImageProcessor = MapImageProcessor.Create(game.Map, mapInfo.ImageColoredRegionsPath,
                    mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath, game.IsFogOfWar);
            }

            simulationFlowHandler = new SimulationFlowHandler(game, mapImageProcessor, null);

            gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            gameMapPictureBox.Image = simulationFlowHandler.ImageProcessor.MapImage;
            gameMapPictureBox.Height = gameMapPictureBox.Image.Height;
            gameMapPictureBox.Width = gameMapPictureBox.Image.Width;
            gameMapPictureBox.BackgroundImage = simulationFlowHandler.ImageProcessor.TemplateImage;

            if (game.IsFogOfWar)
            {
                InitializePlayersPerspectiveComboBox(game.Players);
                playerPerspectiveComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            }
            else
            {
                playerPerspectiveLabel.Hide();
                playerPerspectiveComboBox.Hide();
            }

            this.simulationFlowHandler.OnImageChanged += gameMapPictureBox.Refresh;
        }

        public async void PlayOrStopButtonClick(object sender, EventArgs args)
        {
            var timeForBotMove = new TimeSpan(0, 0,
                0, 0, milliseconds: (int)botThinkingTimeNumericUpDown.Value);
            if (!simulationFlowHandler.IsRunning)
            {
                await simulationFlowHandler.StartOrContinueEvaluationAsync(timeForBotMove);
            }
            else
            {
                simulationFlowHandler.PauseEvaluation();
            }
        }

        public void NextActionButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToNextAction();
            }
        }

        public void PreviousActionButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToPreviousAction();
            }
        }

        public void NextRoundButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToNextRound();
            }
        }

        public void PreviousRoundButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToPreviousRound();
            }
        }

        public void EndOfTheGameButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToEnd();
            }
        }

        public void BeginningOfTheGameButtonClick(object sender, EventArgs args)
        {
            if (!simulationFlowHandler.IsRunning)
            {
                simulationFlowHandler.MoveToBeginning();
            }
        }

        private void PlayerPerspectiveDrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                var comboBoxItem = (ComboBoxItem)((ComboBox) sender)
                    .Items[e.Index];
                var selectedPlayer = simulationFlowHandler.Game.Players.FirstOrDefault(x => x.Id == comboBoxItem.Value);

                Color playerColor;
                playerColor = selectedPlayer == null ? Color.White
                    : Color.FromKnownColor(selectedPlayer.Color);

                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = playerColor;
                Brush b = new SolidBrush(c);
                g.FillRectangle(b, rect.X, rect.Y,
                    rect.Width, rect.Height);
                
                g.DrawString(comboBoxItem.Text, f, Brushes.Black, rect.X, rect.Top);
            }
        }

        private void PlayerPerspectiveChanged(object sender, EventArgs e)
        {
            var typedSender = (ComboBox) sender;
            int selectedIndex = typedSender.SelectedIndex;

            var comboBoxItem = typedSender.Items[selectedIndex] as ComboBoxItem;
            if (comboBoxItem == null)
            {
                return;
            }

            var player = simulationFlowHandler
                .Game.Players.FirstOrDefault(x => x.Id == comboBoxItem.Value);
            
            simulationFlowHandler.ChangePlayerPerspective(player);
        }
    }
}
