﻿namespace WinformsUI.HelperControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.GameUser;
    using GameObjectsLib.Players;

    public partial class HumanPlayerSettingsControl : UserControl
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

        public HumanPlayerSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Adds player to table to the last position.
        /// </summary>
        public void AddPlayer(User user)
        {
            if (PlayersLimit <= PlayersCount)
            {
                throw new ArgumentException();
            }

            HumanPlayerControl control = new HumanPlayerControl
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            }; // TODO: generate unique name
            playersTableLayoutPanel.Controls.Add(control);
        }

        /// <summary>
        ///     Removes the last player from the table.
        /// </summary>
        public void RemovePlayer()
        {
            if (PlayersCount == 0)
            {
                throw new Exception();
            }

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
        ///     Replaces player on given index with given user.
        /// </summary>
        /// <param name="index">Index indicating which row of table will be replaced.</param>
        /// <param name="user">Indicates with whom will the previous user be replaced.</param>
        public void ReplacePlayer(int index, User user)
        {
            HumanPlayerControl control = (HumanPlayerControl) playersTableLayoutPanel.Controls[index];

            Global.MyUser = user;
        }

        /// <summary>
        ///     Calculates and returns players from controls saved in the table.
        /// </summary>
        /// <returns>Returns players from controls saved in the table.</returns>
        public IList<Player> GetPlayers()
        {
            IList<Player> players = new List<Player>();
            foreach (object control in playersTableLayoutPanel.Controls)
            {
                HumanPlayerControl humanPlayerControl = control as HumanPlayerControl;

                players.Add(humanPlayerControl.GetPlayer()); // TODO: may throw exception
            }

            return players;
        }
    }
}
