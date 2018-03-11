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
    using GameHandlersLib.GameHandlers;
    using GameHandlersLib.MapHandlers;
    using GameObjectsLib.Game;

    public partial class SimulatorInGameControl : UserControl
    {
        private SimulationFlowHandler simulationFlowHandler;

        public SimulatorInGameControl()
        {
            InitializeComponent();
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
    }
}
