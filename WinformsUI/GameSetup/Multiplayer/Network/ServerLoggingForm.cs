using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            // local validation
            /*if (loginTextBox.Text.Length < 4 || loginTextBox.Text.Length > 15)
            {
                
            }
            if (passwordTextBox.Text.Length < 4 || passwordTextBox.Text.Length > 50)
            {
                
            }*/
            // server side validation
            var user = new MyNetworkUser(1, loginTextBox.Text); // returned user


            TcpClient client = new TcpClient();
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("78.128.199.63"), 5000);
                client.Connect(endPoint);
            }

            using (var stream = client.GetStream())
            {
                // cant log in, return
                if (!user.LogIn(passwordTextBox.Text, stream)) return;
            }

            User = user;

            // close the form
            Close();

            DialogResult = DialogResult.OK;


        }

    }
}
