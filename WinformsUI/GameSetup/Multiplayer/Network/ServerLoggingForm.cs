using System;
using System.Windows.Forms;
using ConquestObjectsLib;


namespace WinformsUI.GameSetup.Multiplayer.Network
{
    public partial class ServerLoggingForm : Form
    {
        public User User { get; private set; }
        public ServerLoggingForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles cancellation of the token.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, System.EventArgs e)
        {
            // close the form
            Close();

            DialogResult = DialogResult.Cancel;
        }
        
        /// <summary>
        /// Logs user into server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log(object sender, System.EventArgs e)
        {
            // TODO: validate user entries

            // TODO: send it to database

            // close the form
            Close();

            DialogResult = DialogResult.OK;
        }
    }
}
