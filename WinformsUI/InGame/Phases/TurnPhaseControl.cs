namespace WinformsUI.InGame.Phases
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using GameHandlersLib.GameHandlers;
    using GameObjectsLib;
    using GameState = GameHandlersLib.GameHandlers.GameState;
    using Region = GameObjectsLib.GameMap.Region;

    public partial class TurnPhaseControl : UserControl
    {
        private GameFlowHandler gameFlowHandler;
        
        public event Action OnBegin;

        public event Action OnDeploying;
        public event Action OnDeployed;

        public event Action OnAttacking;
        public event Action OnAttacked;

        public event Action OnCommitting;
        public event Action OnCommitted;

        private GameState State
        {
            get { return InGameControl.GameState; }
            set { InGameControl.GameState = value; }
        }


        /// <summary>
        ///     Resets everything from previous stages of this stage.
        ///     Argument is the new state.
        /// </summary>
        public event Action<GameState> OnReset;

        public TurnPhaseControl()
        {
            InitializeComponent();
            Deploying(new object(), new EventArgs());
        }

        public void Initialize(GameFlowHandler gameFlowHandler)
        {
            this.gameFlowHandler = gameFlowHandler;
        }
        
        private void ResetStateHighlight(GameState state)
        {
            switch (state)
            {
                case GameState.Deploying:
                    deployLabel.BackColor = Color.Transparent;
                    break;
                case GameState.Attacking:
                    attackLabel.BackColor = Color.Transparent;
                    break;
                case GameState.Committing:
                    commitLabel.BackColor = Color.Transparent;
                    break;
            }
        }

        private void HighlightCorrectButton(GameState state)
        {
            switch (state)
            {
                case GameState.Deploying:
                    deployLabel.BackColor = Color.GreenYellow;
                    break;
                case GameState.Attacking:
                    attackLabel.BackColor = Color.GreenYellow;
                    break;
                case GameState.Committing:
                    commitLabel.BackColor = Color.GreenYellow;
                    break;
            }
        }

        private void Committing(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
            ResetStateHighlight(State);
            // committing phase
            State = GameState.Committing;
            // highlight
            HighlightCorrectButton(State);
            OnCommitting?.Invoke();
        }

        private void Attacking(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
            ResetStateHighlight(State);
            // committing phase
            if (State > GameState.Attacking)
            {
                OnReset?.Invoke(GameState.Committing);
            }

            State = GameState.Attacking;
            // highlight
            HighlightCorrectButton(State);
            // propagate into outer form
            OnAttacking?.Invoke();
        }

        private void Deploying(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
            ResetStateHighlight(State);

            // committing phase
            if (State > GameState.Deploying)
            {
                gameFlowHandler.ResetAttacking();
            }

            State = GameState.Deploying;

            // highlight button
            HighlightCorrectButton(State);

            OnDeploying?.Invoke();
        }

        private void Next(object sender, EventArgs e)
        {
            if (State == GameState.Committing)
            {
                nextButton.Enabled = false;
            }

            ResetStateHighlight(State);
            State++;
            HighlightCorrectButton(State);

            InvokeStateEvent(State);
        }

        private void InvokeStateEvent(GameState state)
        {
            switch (state)
            {
                case GameState.Deploying:
                    OnDeploying?.Invoke();
                    break;
                case GameState.Attacking:
                    OnAttacking?.Invoke();
                    break;
                case GameState.Committing:
                    OnCommitting?.Invoke();
                    break;
                case GameState.Committed:
                    OnCommitted?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void Repeat(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
            ResetStateHighlight(State);
            State = GameState.Deploying;
            HighlightCorrectButton(State);
            gameFlowHandler.ResetTurn();
            InvokeStateEvent(State);
        }

        public void ResetControl()
        {
            ResetStateHighlight(GameState.Deploying);
            ResetStateHighlight(GameState.Attacking);
            ResetStateHighlight(GameState.Committing);

            Deploying(new object(), new EventArgs());
        }
    }
}
