namespace WinformsUI.HelperControls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Common.Extensions;
    using GameAi.Data;
    using GameObjectsLib;
    using GameObjectsLib.Players;
    using HelperObjects;

    public partial class AiPlayerControl : UserControl
    {
        private AiPlayer player; // dont want to give access to players regions, so I keep it private
        
        public AiPlayerControl()
        {
            InitializeComponent();
            
            player = new AiPlayer(Difficulty.Medium, "PC1", KnownColor.Blue, GameBotType.MonteCarloTreeSearchBot);
            
            colorButton.BackColor = Color.FromKnownColor(player.Color);

            InitializeBotTypeDropdownList();
        }

        private void InitializeBotTypeDropdownList()
        {
            var enumValues = (IEnumerable<Difficulty>)Enum.GetValues(typeof(Difficulty));

            var list = new List<object>();
            foreach (Difficulty gameBotType in enumValues)
            {
                list.Add(new
                {
                    Text = gameBotType.GetDisplayName(),
                    Value = (int)gameBotType
                });
            }

            difficultyComboBox.ValueMember = "Value";
            difficultyComboBox.DisplayMember = "Text";
            difficultyComboBox.DataSource = list;

            difficultyComboBox.SelectedValue = 1;
        }

        /// <summary>
        ///     Accesses Ai player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            set
            {
                player = new AiPlayer(player.Difficulty, player.Name, value, GameBotType.MonteCarloTreeSearchBot);
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
                player = new AiPlayer(value, player.Name, player.Color, GameBotType.MonteCarloTreeSearchBot);
                difficultyComboBox.SelectedIndex = (int) player.Difficulty;
            }
        }

        /// <summary>
        ///     Accesses Ai player name.
        /// </summary>
        public string PlayerName
        {
            get { return player.Name; }
            set
            {
                player = new AiPlayer(player.Difficulty, value, player.Color, GameBotType.MonteCarloTreeSearchBot);
                aiNameLabel.Text = value;
            }
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                {
                    var newColor =
                        Global.PlayerColorPicker.PickNext(PlayerColor);

                    if (newColor != null)
                    {
                        Global.PlayerColorPicker.ReturnColor(PlayerColor);
                        PlayerColor = newColor.Value;
                    }
                    break;
                }
                case MouseButtons.Right:
                {
                    var newColor =
                        Global.PlayerColorPicker.PickPrevious(PlayerColor);

                    if (newColor != null)
                    {
                        Global.PlayerColorPicker.ReturnColor(PlayerColor);
                        PlayerColor = newColor.Value;
                    }
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns copy of the player.
        /// </summary>
        /// <returns></returns>
        public Player GetPlayer()
        {
            return new AiPlayer(player.Difficulty, player.Name, player.Color, GameBotType.MonteCarloTreeSearchBot);
        }

        private void DifficultyChanged(object sender, EventArgs e)
        {
            var selectedDifficulty = (Difficulty)difficultyComboBox.SelectedValue;
            PlayerDifficulty = selectedDifficulty;
        }
    }
}
