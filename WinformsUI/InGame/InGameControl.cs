using System.Drawing;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;
using System.Linq;
using System.Reflection;
using GameObjectsLib.GameMap;

namespace WinformsUI.InGame
{
    public partial class InGameControl : UserControl
    {
        Game game;

        public Game Game
        {
            get { return game; }
            set
            {
                game = value;
                using (var db = new UtilsDbContext())
                {
                    var mapInfo = (from item in db.Maps
                                  where item.Id == game.Map.Id
                                  select item).Single();

                    processor = MapImageProcessor.Create(Game.Map, mapInfo.ImageColoredRegionsPath,
                        mapInfo.ColorRegionsTemplatePath, mapInfo.ImagePath);

                    gameMapPictureBox.Image = processor.MapImage;
                    gameMapPictureBox.BackgroundImage = processor.TemplateImage;
                    

                    Refresh();
                }
            }
        }

        MapImageProcessor processor;

        public InGameControl()
        {
            InitializeComponent();
            

            //gameMapPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            //gameMapPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
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
    }
}
