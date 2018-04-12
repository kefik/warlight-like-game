namespace WinformsUI.HelperControls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Players;
    using HelperObjects;

    public partial class AiPlayerSettingsControl : UserControl
    {
        /// <summary>
        ///     Property giving count of players currently in the table.
        /// </summary>
        public int PlayersCount
        {
            get { return playersTableLayoutPanel.Controls.Count; }
        }

        /// <summary>
        ///     Limit for players number.
        /// </summary>
        public int PlayersLimit { get; set; }

        public AiPlayerSettingsControl()
        {
            InitializeComponent();
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
            AiPlayerControl control = new AiPlayerControl()
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

            AiPlayerControl aiPlayerControl =
                (AiPlayerControl)playersTableLayoutPanel.Controls[PlayersCount - 1];
            Global.PlayerColorPicker.ReturnColor(aiPlayerControl.PlayerColor);
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
                AiPlayerControl aiPlayerControl = control as AiPlayerControl;

                players.Add(aiPlayerControl.GetPlayer() as AiPlayer); // TODO: may throw exception
            }

            return players;
        }
    }
}
