using System;
using System.Windows.Forms;
using GameObjectsLib;
using GameObjectsLib.GameUser;


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
            Close(); // TODO: not necessary prolly

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
            // local validation
            if (loginTextBox.Text.Length < 4 || loginTextBox.Text.Length > 15)
            {
                // error
            }
            if (passwordTextBox.Text.Length < 4 || passwordTextBox.Text.Length > 50)
            {
                // error
            }
            // server side validation
            // TODO: send it to database
            User user = new MyNetworkUser(1, "Bimbinbiribong"); // returned user
            User = user;


            // close the form
            Close();

            DialogResult = DialogResult.OK;
        }
        
    }
}
