﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows.Forms;
using ConquestObjectsLib;
using ConquestObjectsLib.GameMap;
using ConquestObjectsLib.GameMap.Templates;

namespace WinformsUI.HelperControls
{
    public partial class AIPlayerSettingsControl : UserControl
    {
        /// <summary>
        /// Property giving count of players currently in the table.
        /// </summary>
        public int PlayersCount
        {
            get { return playersTableLayoutPanel.Controls.Count; }
        }
        /// <summary>
        /// Limit for players number.
        /// </summary>
        public int PlayersLimit { get; set; }
        
        public AIPlayerSettingsControl()
        {
            InitializeComponent();

        }
        /// <summary>
        /// Adds player to table to the last position.
        /// </summary>
        public void AddPlayer()
        {
            if (PlayersCount >= PlayersLimit) throw new Exception(); // TODO: cannot be added exception

            AIPlayerControl control = new AIPlayerControl("PC")
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true
            }; // TODO: generate unique name
            playersTableLayoutPanel.Controls.Add(control);
        }
        /// <summary>
        /// Removes the last player from the table.
        /// </summary>
        public void RemovePlayer()
        {
            if (PlayersCount == 0) throw new Exception(); // TODO: cannot be removed exception

            playersTableLayoutPanel.Controls.RemoveAt(PlayersCount - 1);
        }
        /// <summary>
        /// Removes player at given index from the table.
        /// </summary>
        /// <param name="index">Index specifying which row to remove.</param>
        public void RemovePlayer(int index)
        {
            if (index >= PlayersCount || index < 0) throw new ArgumentException();

            playersTableLayoutPanel.Controls.RemoveAt(index);
        }
    }
}