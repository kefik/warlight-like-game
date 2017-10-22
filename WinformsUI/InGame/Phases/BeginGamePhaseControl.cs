namespace WinformsUI.InGame.Phases
{
    using System;
    using System.Windows.Forms;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib;

    public partial class BeginGamePhaseControl : UserControl
    {
        private GameFlowHandler gameFlowHandler;

        public event Action OnCommitted;

        public BeginGamePhaseControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes <see cref="BeginGamePhaseControl"/>.
        /// </summary>
        /// <param name="gameFlowHandler"></param>
        public void Initialize(GameFlowHandler gameFlowHandler)
        {
            this.gameFlowHandler = gameFlowHandler;
        }

        private void Commit(object sender, EventArgs e)
        {
            try
            {
                gameFlowHandler.Commit();
                OnCommitted?.Invoke();
                commitButton.Enabled = false;
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void StartOver(object sender, EventArgs e)
        {
            commitButton.Enabled = true;
            gameFlowHandler.ResetTurn();
        }

        public void ResetControl()
        {
            commitButton.Enabled = true;
        }
    }
}
