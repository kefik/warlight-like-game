namespace WinformsUI.GameSetup.Multiplayer.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjectsLib;
    using GameObjectsLib.Players;
    using HelperControls;

    public partial class NetworkNewGameSettingsControl : UserControl
    {
        private readonly MyHumanPlayerControl myPlayerControl;

        /// <summary>
        ///     Represents number of total players that can play this given map.
        /// </summary>
        private int TotalPlayersLimit
        {
            get { return mapSettingsControl.PlayersLimit; }
        }

        public NetworkNewGameSettingsControl()
        {
            InitializeComponent();

            myPlayerControl = new MyHumanPlayerControl
            {
                Parent = myUserPanel,
                Dock = DockStyle.Fill
            };
            myPlayerControl.Show();

            // TODO: get the code better
            mapSettingsControl.OnMapChosen += (sender, args) =>
            {
                int maxOtherPlayers = Math.Max(0, TotalPlayersLimit - 1);

                aiPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;
                humanPlayersNumberNumericUpDown.Maximum = maxOtherPlayers;

                aiPlayerSettingsControl.PlayersLimit = maxOtherPlayers;
            };
            // initialize
            int maxPlayers = Math.Max(0, TotalPlayersLimit - 1);
            aiPlayersNumberNumericUpDown.Maximum = maxPlayers;
            humanPlayersNumberNumericUpDown.Maximum = maxPlayers;

            aiPlayerSettingsControl.PlayersLimit = maxPlayers;

            previousAiPlayersNumber = aiPlayersNumberNumericUpDown.Value;
            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        public event Func<HumanPlayer, ICollection<AiPlayer>, string, int, Task> OnGameCreated;

        private decimal previousAiPlayersNumber;

        private void OnNumberOfAiPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(aiPlayersNumberNumericUpDown.Value, previousHumanPlayersNumber))
            {
                aiPlayersNumberNumericUpDown.Value = previousAiPlayersNumber;
                return;
            }

            decimal difference;
            if (previousAiPlayersNumber < aiPlayersNumberNumericUpDown.Value)
            {
                difference = aiPlayersNumberNumericUpDown.Value - previousAiPlayersNumber;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.AddPlayer();
                }
            }
            else
            {
                difference = previousAiPlayersNumber - aiPlayersNumberNumericUpDown.Value;
                for (int i = 0; i < difference; i++)
                {
                    aiPlayerSettingsControl.RemovePlayer();
                }
            }

            previousAiPlayersNumber = aiPlayersNumberNumericUpDown.Value;
        }

        private decimal previousHumanPlayersNumber;

        private void OnNumberOfHumanPlayersChanged(object sender, EventArgs e)
        {
            if (!DoesSumToPlayersLimit(humanPlayersNumberNumericUpDown.Value, previousAiPlayersNumber))
            {
                humanPlayersNumberNumericUpDown.Value = previousHumanPlayersNumber;
                return;
            }

            decimal difference;
            if (previousHumanPlayersNumber < humanPlayersNumberNumericUpDown.Value)
            {
                difference = humanPlayersNumberNumericUpDown.Value - previousHumanPlayersNumber;
            }
            else
            {
                difference = previousHumanPlayersNumber - humanPlayersNumberNumericUpDown.Value;
            }

            previousHumanPlayersNumber = humanPlayersNumberNumericUpDown.Value;
        }

        private bool DoesSumToPlayersLimit(decimal aiPlayers, decimal humanPlayers)
        {
            return aiPlayers + humanPlayers <= Math.Max(0, TotalPlayersLimit - 1);
        }

        private async void Create(object sender, EventArgs e)
        {
            if (OnGameCreated == null)
            {
                return;
            }

            IList<AiPlayer> aiPlayers = aiPlayerSettingsControl.GetPlayers();
            await OnGameCreated.Invoke(myPlayerControl.GetPlayer(),
                aiPlayers,
                mapSettingsControl.MapName,
                aiPlayerSettingsControl.PlayersLimit - 1 - aiPlayers.Count);
        }
    }
}
