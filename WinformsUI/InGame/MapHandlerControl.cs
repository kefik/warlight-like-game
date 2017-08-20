using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using WinformsUI.InGame.Phases;
using Region = GameObjectsLib.GameMap.Region;

namespace WinformsUI.InGame
{
    using GameObjectsLib.Game;

    public partial class MapHandlerControl : UserControl
    {
        MapImageProcessor processor;

        public MapImageProcessor Processor
        {
            get { return processor; }
            set
            {
                processor = value;
                if (processor != null)
                {
                    gameMapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                    gameMapPictureBox.Image = processor.MapImage;
                    gameMapPictureBox.BackgroundImage = processor.TemplateImage;
                }
            }
        }

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
        

        
        private void ImageClick(object sender, MouseEventArgs e)
        {
            var region = Processor.GetRegion(e.X, e.Y);
            var state = GetState();
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
        /// Deploys region and its army graphically.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="army">Army that will occupy the region.</param>
        public void Deploy(Region region, int army)
        {
            processor.Recolor(region, region.Owner.Color);
            processor.DrawArmyNumber(region, army);
            RefreshImage();
        }

        Region previouslySelectedRegion;
        void AttackPhaseSelectAttempt(Region region)
        {
            // didnt select any correct region
            if (region == null)
            {
                // i did with last pick tho
                if (previouslySelectedRegion != null)
                {
                    // unhighlight my last pick
                    processor.UnhighlightRegion(previouslySelectedRegion, GetPlayerOnTurn(), GetRegionArmy(previouslySelectedRegion));
                }
                // reset
                previouslySelectedRegion = null;
                return;
            }
            int realArmy = GetRegionArmy(region);

            // this is my first correctly selected region
            if (previouslySelectedRegion == null)
            {
                if (region?.Owner != GetPlayerOnTurn()) return;

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
                processor.UnhighlightRegion(previouslySelectedRegion, GetPlayerOnTurn(), GetRegionArmy(previouslySelectedRegion));
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
        /// Seizes the region for the concrete player, recoloring it properly.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="playerOnTurn"></param>
        public void SeizeRegion(Region region, Player playerOnTurn)
        {
            if (GetState() != GameState.GameBeginning) return;

            processor.Recolor(region, playerOnTurn.Color);
            RefreshImage();
        }


    }
}
