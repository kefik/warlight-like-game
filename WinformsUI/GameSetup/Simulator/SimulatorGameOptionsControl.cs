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
            add { simulatorNewGameSettingsControl1.OnSimulationStarted += value; }
            remove { simulatorNewGameSettingsControl1.OnSimulationStarted -= value; }
        }

        public SimulatorGameOptionsControl()
        {
            InitializeComponent();
        }
    }
}
