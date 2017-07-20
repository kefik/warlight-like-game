using System.Windows.Forms;
using GameObjectsLib.Game;

namespace WinformsUI.InGame
{
    public partial class InGameControl : UserControl
    {
        public Game Game { get; set; }
        public InGameControl()
        {
            InitializeComponent();
        }
    }
}
