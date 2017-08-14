﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib.GameUser;
using GameObjectsLib;

namespace WinformsUI.HelperControls
{
    public partial class MyHumanPlayerControl : UserControl
    {
        public MyHumanPlayerControl()
        {
            InitializeComponent();

            // initialize
            playerNameTextBox.Text = Global.MyUser.Name;
            playerNameTextBox.Enabled = true;
            PlayerColor = KnownColor.Green;
        }
        
        KnownColor playerColor;
        /// <summary>
        /// Represents clients color.
        /// </summary>
        public KnownColor PlayerColor
        {
            get { return playerColor; }
            private set
            {
                playerColor = value;
                colorButton.BackColor = System.Drawing.Color.FromKnownColor(PlayerColor);
            }
        }

        /// <summary>
        /// Represents name of player this control is representing.
        /// </summary>
        public string PlayerName
        {
            get { return Global.MyUser.Name; }
            private set
            {
                Global.MyUser.Name = value;
                playerNameTextBox.Text = value;
            }
        }


        /// <summary>
        /// Creates and returns new instance of player represented by this control.
        /// </summary>
        /// <returns>New instance of the player represented by this control.</returns>
        public HumanPlayer GetPlayer()
        {
            return new HumanPlayer(Global.MyUser, PlayerColor);
        }

        private void ChangeColor(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if ((int) PlayerColor >= 173) PlayerColor = (KnownColor) 0;
                    else PlayerColor++;
                    break;
                case MouseButtons.Right:
                    if ((int)PlayerColor <= 0) PlayerColor = (KnownColor)173;
                    else PlayerColor--;
                    break;
            }
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            switch (Global.MyUser.UserType)
            {
                case UserType.LocalUser:
                    Global.MyUser.Name = Name;
                    break;
                case UserType.NetworkUser:
                    playerNameTextBox.Text = PlayerName;
                    break;
            }
        }
    }
}
