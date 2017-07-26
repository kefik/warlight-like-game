﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameObjectsLib;
using Region = GameObjectsLib.GameMap.Region;

namespace WinformsUI.InGame.Phases
{
    public partial class TurnPhaseControl : UserControl
    {
        GameState state = GameState.Deploying;
        public Deploying DeployingStructure { get; } = new Deploying(new List<Tuple<Region, int>>());
        public Attacking AttackingStructure { get; } = new Attacking(new List<Attack>());

        public TurnPhaseControl()
        {
            InitializeComponent();

            Deploying(new object(), new EventArgs());
        }

        /// <summary>
        ///     Is invoked when state of the game is changed.
        /// Argument is the new state.
        /// </summary>
        public event Action<GameState> OnStateChanged;

        /// <summary>
        ///     Resets everything from previous stages of this stage.
        /// Argument is the new state.
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

        void Committing(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            // committing phase
            state = GameState.Committing;
            // highlight
            HighlightCorrectButton(state);
            OnStateChanged?.Invoke(state);
        }

        void Attacking(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            // committing phase
            if (state > GameState.Attacking) OnReset?.Invoke(GameState.Committing);

            state = GameState.Attacking;
            // highlight
            HighlightCorrectButton(state);
            // propagate into outer form
            OnStateChanged?.Invoke(state);
        }

        void Deploying(object sender, EventArgs e)
        {
            ResetStateHighlight(state);

            // committing phase
            if (state > GameState.Deploying) OnReset?.Invoke(GameState.Attacking);

            state = GameState.Deploying;
            
            // highlight button
            HighlightCorrectButton(state);

            OnStateChanged?.Invoke(state);
        }

        void Next(object sender, EventArgs e)
        {
            if (state >= GameState.Committed) return;
            ResetStateHighlight(state);

            state++;
            HighlightCorrectButton(state);
            OnStateChanged?.Invoke(state);
        }

        void Repeat(object sender, EventArgs e)
        {
            ResetStateHighlight(state);
            state = GameState.Deploying;
            HighlightCorrectButton(state);
            OnReset?.Invoke(state);
            OnStateChanged?.Invoke(state);
        }

        /// <summary>
        ///     Returns army currently occuppying the region.
        /// </summary>
        /// <param name="region">Region.</param>
        /// <returns>Army.</returns>
        public int GetRealArmy(Region region)
        {
            return AttackingStructure.GetUnitsLeftToAttack(region, DeployingStructure) + 1;
        }
    }
}
