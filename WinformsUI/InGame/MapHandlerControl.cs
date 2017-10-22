namespace WinformsUI.InGame
{
    using System;
    using System.Windows.Forms;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;
    using Phases;
    using GameState = GameHandlersLib.GameHandlers.GameState;

    public partial class MapHandlerControl : UserControl
    {
        private GameFlowHandler gameFlowHandler;

        public MapHandlerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes <see cref="MapHandlerControl"/> instance.
        /// </summary>
        /// <param name="gameFlowHandler"></param>
        public void Initialize(GameFlowHandler gameFlowHandler)
        {
            this.gameFlowHandler = gameFlowHandler;
            if (gameFlowHandler == null)
            {
                throw new ArgumentException();
            }
            gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            gameMapPictureBox.Image = gameFlowHandler.ImageProcessor.MapImage;
            gameMapPictureBox.Height = gameMapPictureBox.Image.Height;
            gameMapPictureBox.Width = gameMapPictureBox.Image.Width;
            gameMapPictureBox.BackgroundImage = gameFlowHandler.ImageProcessor.TemplateImage;

            this.gameFlowHandler.OnImageChanged += gameMapPictureBox.Refresh;
        }

        private void ImageClick(object sender, MouseEventArgs e)
        {
            GameState state = InGameControl.GameState;
            if (state == GameState.Deploying)
            {
                try
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            gameFlowHandler.Deploy(e.X, e.Y, 1);
                            break;
                        case MouseButtons.Right:
                            gameFlowHandler.Deploy(e.X, e.Y, -1);
                            break;
                    }
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else if (state == GameState.Attacking)
            {
                int selectedRegionsCount = gameFlowHandler.Select(e.X, e.Y);
                if (selectedRegionsCount == 2)
                {
                    AttackManagerForm attackManager = new AttackManagerForm
                    {
                        ArmyLowerLimit = 0,
                        ArmyUpperLimit
                            = gameFlowHandler.GetUnitsLeftToAttackInAttackingRegion()
                    };
                    DialogResult dialogResult = attackManager.ShowDialog();
                    // execute the attack
                    if (dialogResult == DialogResult.OK)
                    {
                        gameFlowHandler.Attack(attackManager.AttackingArmy);
                    }
                }
            }
            else if (state == GameState.GameBeginning)
            {
                try
                {
                    gameFlowHandler.Seize(e.X, e.Y);
                }
                catch (ArgumentOutOfRangeException error)
                {
                    MessageBox.Show(error.Message);
                }
                catch (ArgumentException error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void ImageHover(object sender, MouseEventArgs e)
        {
            // TODO: fix flickering
            //if (processor.GetRegion(e.X, e.Y) != null) Cursor.Current = Cursors.Hand;
            //else Cursor.Current = Cursors.Default;
        }
    }
}
