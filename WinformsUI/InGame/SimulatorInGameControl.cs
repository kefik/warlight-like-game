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

            simulationFlowHandler = new SimulationFlowHandler(game, mapImageProcessor);

            gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            gameMapPictureBox.Image = simulationFlowHandler.ImageProcessor.MapImage;
            gameMapPictureBox.Height = gameMapPictureBox.Image.Height;
            gameMapPictureBox.Width = gameMapPictureBox.Image.Width;
            gameMapPictureBox.BackgroundImage = simulationFlowHandler.ImageProcessor.TemplateImage;

            this.simulationFlowHandler.OnImageChanged += gameMapPictureBox.Refresh;
        }
    }
}
