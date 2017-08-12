﻿using System;
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
            /*if (loginTextBox.Text.Length < 3 || loginTextBox.Text.Length > 15)
            {
                
            }
            if (passwordTextBox.Text.Length < 4 || passwordTextBox.Text.Length > 50)
            {
                
            }*/
            // server side validation
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("78.128.199.63"), 5000);
            TcpClient client = new TcpClient();
            {
#if (!DEBUG)
                try
                {
#endif
                    client.Connect(endPoint);
#if (!DEBUG)
                }
                catch (SocketException)
                {
                    MessageBox.Show("Server is unavailable, please, contact the administrator.");
                    return;
                }
#endif
            }
            var user = new MyNetworkUser(loginTextBox.Text, client, endPoint); // returned user

            // cant log in, return
            if (!user.LogIn(passwordTextBox.Text))
            {
                MessageBox.Show("Invalid credentials!");
                return;
            }

            User = user;

            // close the form
            Close();

            DialogResult = DialogResult.OK;


        }

    }
}
