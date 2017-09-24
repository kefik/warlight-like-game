namespace WinformsUI.InGame
{
    using System;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Game;
    using GameObjectsLib.GameMap;

    public partial class MapHandlerControl : UserControl
    {
        private MapImageProcessor processor;

        public event Action<Region> OnRegionSeizeAttempt;

        public Func<GameState> GetState;
        public Func<Player> GetPlayerOnTurn;
        public Func<Region, int> GetRegionArmy;

        public event Action<Region, int> OnDeployAttempt;
        public event Action<Region, Region> OnAttackAttempt;

        public MapHandlerControl()
        {
            InitializeComponent();
        }

        public void Initialize(MapImageProcessor processor)
        {
            this.processor = processor;
            if (processor == null)
            {
                throw new ArgumentException();
            }
            gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            gameMapPictureBox.Image = processor.MapImage;
            gameMapPictureBox.Height = gameMapPictureBox.Image.Height;
            gameMapPictureBox.Width = gameMapPictureBox.Image.Width;
            gameMapPictureBox.BackgroundImage = processor.TemplateImage;
        }

        private void ImageClick(object sender, MouseEventArgs e)
        {
            Region region = processor.GetRegion(e.X, e.Y);
            GameState state = GetState();
            if (state == GameState.Deploying)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        OnDeployAttempt?.Invoke(region, 1);
                        break;
                    case MouseButtons.Right:
                        OnDeployAttempt?.Invoke(region, -1);
                        break;
                }
            }
            else if (state == GameState.Attacking)
            {
                AttackPhaseSelectAttempt(region);
            }
            else if (state == GameState.GameBeginning)
            {
                OnRegionSeizeAttempt?.Invoke(region);
            }
        }

        private void ImageHover(object sender, MouseEventArgs e)
        {
            // TODO: fix flickering
            //if (processor.GetRegion(e.X, e.Y) != null) Cursor.Current = Cursors.Hand;
            //else Cursor.Current = Cursors.Default;
        }

        public void RefreshImage()
        {
            gameMapPictureBox.Refresh();
        }

        public void StartOver(GameBeginningRound gameBeginningRound)
        {
            processor.ResetRound(gameBeginningRound);
            RefreshImage();
        }

        public void StartOver(GameRound gameRound)
        {
            processor.ResetRound(gameRound);
            RefreshImage();
        }

        /// <summary>
        ///     Deploys region and its army graphically.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="army">Army that will occupy the region.</param>
        public void Deploy(Region region, int army)
        {
            processor.Recolor(region, region.Owner.Color);
            processor.DrawArmyNumber(region, army);
            RefreshImage();
        }

        private Region previouslySelectedRegion;

        private void AttackPhaseSelectAttempt(Region region)
        {
            // didnt select any correct region
            if (region == null)
            {
                // i did with last pick tho
                if (previouslySelectedRegion != null)
                {
                    // unhighlight my last pick
                    processor.UnhighlightRegion(previouslySelectedRegion, GetPlayerOnTurn(),
                        GetRegionArmy(previouslySelectedRegion));
                }
                // reset
                previouslySelectedRegion = null;
                return;
            }
            int realArmy = GetRegionArmy(region);

            // this is my first correctly selected region
            if (previouslySelectedRegion == null)
            {
                if (region?.Owner != GetPlayerOnTurn())
                {
                    return;
                }

                processor.HighlightRegion(region, realArmy);
            }
            else if (previouslySelectedRegion != null
                     && previouslySelectedRegion.IsNeighbourOf(region))
            {
                // highlight the region
                processor.HighlightRegion(region, realArmy);
                RefreshImage();
                // calls attack attempt
                OnAttackAttempt?.Invoke(previouslySelectedRegion, region);
                // unhighlight what u highlighted
                processor.UnhighlightRegion(previouslySelectedRegion, GetPlayerOnTurn(),
                    GetRegionArmy(previouslySelectedRegion));
                processor.UnhighlightRegion(region, GetPlayerOnTurn(), GetRegionArmy(region));
                // reset
                region = null;
                previouslySelectedRegion = null;
            }
            else
            {
                processor.UnhighlightRegion(region, GetPlayerOnTurn(),
                    GetRegionArmy(region));
                region = null;

                processor.UnhighlightRegion(previouslySelectedRegion, GetPlayerOnTurn()
                    , GetRegionArmy(previouslySelectedRegion));
                previouslySelectedRegion = null;
            }
            previouslySelectedRegion = region;
            RefreshImage();
        }

        /// <summary>
        ///     Seizes the region for the concrete player, recoloring it properly.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="playerOnTurn"></param>
        public void SeizeRegion(Region region, Player playerOnTurn)
        {
            if (GetState() != GameState.GameBeginning)
            {
                return;
            }

            processor.Recolor(region, playerOnTurn.Color);
            RefreshImage();
        }

        public void ResetRound(Round round)
        {
            if (round.GetType() == typeof(GameBeginningRound))
            {
                processor.ResetRound((GameBeginningRound) round);
            }
            else
            {
                processor.ResetRound((GameRound) round);
            }
        }

        public void Refresh(Game game)
        {
            processor.Refresh(game);
            RefreshImage();
        }

        public void ResetAttackingPhase(Attacking attackingPhase, Deploying deployingPhase)
        {
            processor.ResetAttackingPhase(attackingPhase, deployingPhase);
        }

        public void ResetDeployingPhase(Deploying deployingPhase)
        {
            processor.ResetDeployingPhase(deployingPhase);
        }
    }
}
