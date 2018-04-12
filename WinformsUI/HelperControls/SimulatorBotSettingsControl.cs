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
    using GameObjectsLib.Players;

    public partial class SimulatorBotSettingsControl : UserControl
    {
        /// <summary>
        ///     Limit for players number.
        /// </summary>
        public int PlayersLimit { get; set; }
        
        public SimulatorBotSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Property giving count of players currently in the table.
        /// </summary>
        public int PlayersCount
        {
            get { return playersTableLayoutPanel.Controls.Count; }
        }

        /// <summary>
        ///     Adds player to table to the last position.
        /// </summary>
        public void AddPlayer()
        {
            if (PlayersCount >= PlayersLimit)
            {
                throw new ArgumentException();
            }

            var colorToPick = Global.PlayerColorPicker.PickAny() ?? throw new ArgumentException("All colors depleted.");
            SimulatorBotPlayerControl control = new SimulatorBotPlayerControl()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                PlayerName = $"PC{playersTableLayoutPanel.Controls.Count + 1}",
                PlayerColor = colorToPick
            };
            playersTableLayoutPanel.Controls.Add(control);
        }

        /// <summary>
        ///     Removes the last player from the table.
        /// </summary>
        public void RemovePlayer()
        {
            if (PlayersCount == 0)
            {
                throw new ArgumentException();
            }

            SimulatorBotPlayerControl botPlayerControl =
                (SimulatorBotPlayerControl)playersTableLayoutPanel.Controls[PlayersCount - 1];
            Global.PlayerColorPicker.ReturnColor(botPlayerControl.PlayerColor);
            playersTableLayoutPanel.Controls.RemoveAt(PlayersCount - 1);
        }

        /// <summary>
        ///     Removes player at given index from the table.
        /// </summary>
        /// <param name="index">Index specifying which row to remove.</param>
        public void RemovePlayer(int index)
        {
            if (index >= PlayersCount || index < 0)
            {
                throw new ArgumentException();
            }

            playersTableLayoutPanel.Controls.RemoveAt(index);
        }

        /// <summary>
        ///     Calculates and returns players from controls saved in the table.
        /// </summary>
        /// <returns>Returns player from controls saved in the table.</returns>
        public IList<AiPlayer> GetPlayers()
        {
            IList<AiPlayer> players = new List<AiPlayer>();
            foreach (object control in playersTableLayoutPanel.Controls)
            {
                SimulatorBotPlayerControl aiPlayerControl = control as SimulatorBotPlayerControl;

                players.Add(aiPlayerControl.GetPlayer() as AiPlayer); // TODO: may throw exception
            }

            return players;
        }
    }
}
