using System;
using System.Drawing;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;
using System.Linq;
using System.Reflection;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using WinformsUI.InGame.Phases;

namespace WinformsUI.InGame
{
    public partial class InGameControl : UserControl
    {
        Game game;
        MapImageProcessor processor;

        BeginRoundPhaseControl beginRoundPhaseControl;
        TurnPhaseControl turnPhaseControl;
        WatchTurnPhaseControl watchTurnPhaseControl;
        
        public Game Game
        {
            get { return game; }
            set
            {
                if (game != null) throw new ArgumentException();

                game = value;
                state = game.RoundNumber == 0 ? GameState.GameBeginning : GameState.RoundBeginning;
                using (var db = new UtilsDbContext())
                {
                    var mapInfo = (from item in db.Maps
                                  where item.Id == game.Map.Id
                                  select item).Single();

                    processor = MapImageProcessor.Create(Game.Map, mapInfo.ImageColoredRegionsPath,
                        mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath);

                    processor.Refresh(value);

                    gameMapPictureBox.Image = processor.MapImage;
                    gameMapPictureBox.BackgroundImage = processor.TemplateImage;
                    
                    Refresh();
                }
            }
        }

        GameState state;

        void RefreshImage()
        {
            gameMapPictureBox.Image = processor.MapImage;
        }


        

        public InGameControl()
        {
            InitializeComponent();

            gameMapPictureBox.BackgroundImageLayout = ImageLayout.None;
        }

        private void ImageClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(processor.GetRegion(e.X, e.Y)?.ToString());
        }

        private void ImageHover(object sender, MouseEventArgs e)
        {
            // TODO: fix flickering
            //if (processor.GetRegion(e.X, e.Y) != null) Cursor.Current = Cursors.Hand;
            //else Cursor.Current = Cursors.Default;
            
        }

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;

            if (pictureBox == null) return;
        }

        void NextPhase()
        {
            
        }
        
    }
}
