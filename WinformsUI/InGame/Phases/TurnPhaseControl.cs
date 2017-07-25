﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameObjectsLib;
using Region = GameObjectsLib.GameMap.Region;

namespace WinformsUI.InGame.Phases
{
    public partial class TurnPhaseControl : UserControl
    {
        GameState state = GameState.Deploying;
        public Deploying DeployingStructure { get; } = new Deploying(new List<Tuple<Region, int>>());
        public Attacking AttackingStructure { get; }  = new Attacking(new List<Attack>());

        public TurnPhaseControl()
        {
            InitializeComponent();

            Deploying(new object(), new EventArgs());
        }
        /// <summary>
        /// Is invoked when state of the game is changed.
        /// </summary>
        public event Action<GameState> OnStateChanged;
        /// <summary>
        /// Resets everything from previous stages of this stage.
        /// </summary>
        public event Action<GameState> OnReset;

        void ResetStateHighlight(GameState state)
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

        void HighlightCorrectButton(GameState state)
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
            ResetStateHighlight(state);
            // committing phase
            state = GameState.Committing;
            // highlight
            HighlightCorrectButton(state);
            OnStateChanged?.Invoke(state);
        }

        private void Attacking(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            // committing phase
            if (state > GameState.Attacking) OnReset?.Invoke(GameState.Attacking);

            state = GameState.Attacking;
            // highlight
            HighlightCorrectButton(state);
            // propagate into outer form
            OnStateChanged?.Invoke(state);
        }

        private void Deploying(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            // committing phase
            state = GameState.Deploying;
            // highlight button
            HighlightCorrectButton(state);

            OnStateChanged?.Invoke(state);
        }

        private void Next(object sender, EventArgs e)
        {
            if (state >= GameState.Committed) return;
            ResetStateHighlight(state);
            
            state++;
            HighlightCorrectButton(state);
            OnStateChanged?.Invoke(state);
        }

        private void Repeat(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            state = GameState.Deploying;
            HighlightCorrectButton(state);
            OnReset?.Invoke(state);
            OnStateChanged?.Invoke(state);
        }
    }
}
