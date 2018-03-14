using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.HelperControls
{
    using Common.Extensions;
    using GameAi.Data;
    using GameObjectsLib;
    using GameObjectsLib.Players;

    public partial class SimulatorBotPlayerControl : UserControl
    {
        private AiPlayer player; // dont want to give access to players regions, so I keep it private

        public SimulatorBotPlayerControl(string uniqueName)
        {
            InitializeComponent();
            
            InitializeBotTypeDropdownList();

            player = new AiPlayer(Difficulty.Hard,
                uniqueName, KnownColor.Blue,
                GameBotType.MonteCarloTreeSearchBot);

            PlayerColor = player.Color;
            BotType = GameBotType.AggressiveBot;
            PlayerName = player.Name;
        }

        public SimulatorBotPlayerControl(AiPlayer player)
        {
            InitializeComponent();

            this.player = player;

            InitializeBotTypeDropdownList();
        }

        private void InitializeBotTypeDropdownList()
        {
            var enumValues = (IEnumerable<GameBotType>)Enum.GetValues(typeof(GameBotType));

            var list = new List<object>();
            foreach (GameBotType gameBotType in enumValues)
            {
                list.Add(new
                {
                    Text = gameBotType.GetDisplayName(),
                    Value = (int) gameBotType
                });
            }

            botTypeComboBox.ValueMember = "Value";
            botTypeComboBox.DisplayMember = "Text";
            botTypeComboBox.DataSource = list;
        }

        /// <summary>
        ///     Accesses Ai player color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return player.Color; }
            private set
            {
                player = new AiPlayer(player.Difficulty, player.Name, value, player.BotType);
                colorButton.BackColor = Color.FromKnownColor(value);
            }
        }

        /// <summary>
        ///     Accesses Ai player difficulty.
        /// </summary>
        public GameBotType BotType
        {
            get { return player.BotType; }
            private set
            {
                player = new AiPlayer(player.Difficulty, player.Name, player.Color, value);
                botTypeComboBox.SelectedIndex = (int)player.BotType;
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
                player = new AiPlayer(player.Difficulty, value, player.Color, player.BotType);
                aiNameLabel.Text = value;
            }
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int)PlayerColor >= 173)
                    {
                        PlayerColor = 0;
                    }
                    else
                    {
                        PlayerColor++;
                    }
                    break;
                case MouseButtons.Right:
                    if ((int)PlayerColor <= 0)
                    {
                        PlayerColor = (KnownColor)173;
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
            return new AiPlayer(player.Difficulty, player.Name, player.Color, player.BotType);
        }
    }
}
