namespace WinformsUI.HelperControls
{
    using System.Drawing;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Player;

    public partial class AiPlayerControl : UserControl
    {
        private AiPlayer player; // dont want to give access to players regions, so I keep it private

        public AiPlayerControl(string uniqueName)
        {
            InitializeComponent();

            player = new AiPlayer(Difficulty.Medium, "PC1", KnownColor.Blue);

            difficultyComboBox.SelectedIndex = (int) player.Difficulty;
            colorButton.BackColor = Color.FromKnownColor(player.Color);
        }

        public AiPlayerControl(AiPlayer player)
        {
            InitializeComponent();

            this.player = player;
        }

        /// <summary>
        ///     Accesses Ai player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            private set
            {
                player = new AiPlayer(player.Difficulty, player.Name, value);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }

        /// <summary>
        ///     Accesses Ai player difficulty.
        /// </summary>
        public Difficulty PlayerDifficulty
        {
            get { return player.Difficulty; }
            private set
            {
                player = new AiPlayer(value, player.Name, player.Color);
                difficultyComboBox.SelectedIndex = (int) player.Difficulty;
            }
        }

        /// <summary>
        ///     Accesses Ai player name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
            private set
            {
                player = new AiPlayer(player.Difficulty, value, player.Color);
                aiNameLabel.Text = value;
            }
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int) PlayerColor >= 173)
                    {
                        PlayerColor = 0;
                    }
                    else
                    {
                        PlayerColor++;
                    }
                    break;
                case MouseButtons.Right:
                    if ((int) PlayerColor <= 0)
                    {
                        PlayerColor = (KnownColor) 173;
                    }
                    else
                    {
                        PlayerColor--;
                    }
                    break;
            }
        }

        /// <summary>
        ///     Returns copy of the player.
        /// </summary>
        /// <returns></returns>
        public Player GetPlayer()
        {
            return new AiPlayer(player.Difficulty, player.Name, player.Color);
        }
    }
}
