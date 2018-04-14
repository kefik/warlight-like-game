using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsUI.GameSetup.Simulator
{
    using GameObjectsLib.Game;

    public partial class SimulatorGameOptionsControl : UserControl
    {
        public event Action<Game> OnSimulationStarted
        {
            add { simulatorNewGameSettingsControl.OnSimulationStarted += value; }
            remove { simulatorNewGameSettingsControl.OnSimulationStarted -= value; }
        }

        public event Action<Game> OnSimulationLoaded
        {
            add { simulatorLoadGamesControl.OnSimulationLoaded += value; }
            remove { simulatorLoadGamesControl.OnSimulationLoaded -= value; }
        }

        public SimulatorGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
