﻿using System.Windows.Forms;

namespace WinformsUI.GameSetup.Multiplayer
{
    public partial class GameTypeChoiceForm : Form
    {
        public GameTypeChoiceForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, System.EventArgs e)
        {
            Close();
        }

        private void ChooseType(object sender, System.EventArgs e)
        {
            // TODO: load proper screen

            Close();
        }
    }
}
