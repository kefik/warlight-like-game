using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectsLib.Game
{
    using GameMap;
    using GameUser;

    public enum GameState : byte
    {
        GameBeginning,
        RoundBeginning,
        Deploying,
        Attacking,
        Committing,
        Committed,
        GameEnd
    }
    /// <summary>
    /// Component handling game state changes and all reactions to them.
    /// </summary>
    public abstract class GameFlowHandler
    {
        public event Action OnGameBeginning;
        public event Action OnRoundBeginning;
        public event Action OnDeploying;
        public event Action OnAttacking;
        public event Action OnCommitting;
        public event Action OnCommitted;
        public event Action OnGameEnd;

        public event Action OnPlayRound;

        public Game Game { get; }

        public IList<GameRound> Rounds { get; set; } = new List<GameRound>();

        protected GameFlowHandler(Game game)
        {
            Game = game;
        }

        protected GameState gameState;
        public virtual void GameStateChanged(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.GameBeginning:
                    OnGameBeginning?.Invoke();
                    break;
                case GameState.RoundBeginning:
                    OnRoundBeginning?.Invoke();
                    break;
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
                case GameState.GameEnd:
                    OnGameEnd?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            }
            gameState = newGameState;
        }


        /// <summary>
        /// Plays given initial round, refreshing the situation of the game.
        /// </summary>
        /// <param name="round">Round to be played.</param>
        public virtual void PlayRound(GameBeginningRound round)
        {
            foreach (var roundSelectedRegion in round.SelectedRegions)
            {
                var realRegion = (from region in Game.Map.Regions
                                  where region == roundSelectedRegion.Item2
                                  select region).First();
                var realPlayer = (from player in Game.Players
                                  where player == roundSelectedRegion.Item1
                                  select player).First();

                realRegion.Owner = realPlayer;
            }

            Game.Refresh();
            Game.RoundNumber++;
        }

        /// <summary>
        /// Plays given round, calculating everything, moving this instance of
        /// the game into position after the round was played.
        /// </summary>
        public virtual void PlayRound()
        {
            void PlayDeploying(GameRound round)
            {
                var deploying = round.Deploying;
                foreach (var deployedArmies in deploying.ArmiesDeployed)
                {
                    var region = (from item in Game.Map.Regions
                                  where item == deployedArmies.Item1
                                  select item).First();
                    region.Army = deployedArmies.Item2;
                }
            }
            void PlayAttacking(GameRound round)
            {
                var attacks = round.Attacking.Attacks;
                foreach (var attack in attacks)
                {
                    // TODO: real calculation according to rules
                    // get real attacker
                    var attacker = (from region in Game.Map.Regions
                                    where region == attack.Attacker
                                    select region).First();

                    // if attacking region changed owner, cancel attack
                    if (attacker.Owner != attack.Attacker.Owner) continue;

                    // get real defender
                    var defender = (from region in Game.Map.Regions
                                    where region == attack.Defender
                                    select region).First();
                    // situation might have changed => recalculate attacking army
                    int realAttackingArmy = Math.Min(attack.AttackingArmy, attacker.Army);
                    // if they have same owner == just moving armies
                    if (defender.Owner == attacker.Owner)
                    {
                        // sum armies
                        defender.Army += realAttackingArmy;
                        // units were transfered
                        attacker.Army -= realAttackingArmy;
                    }
                    // attacking
                    else
                    {
                        Random random = new Random();

                        // calculate how many defending units were killed
                        int defendingArmyUnitsKilled = 0;
                        for (int i = 0; i < realAttackingArmy && defendingArmyUnitsKilled < defender.Army; i++)
                        {
                            double attackingUnitWillKillPercentage = random.Next(100) / 100d;

                            // attacking unit has 60% chance to kill defending unit
                            bool attackingUnitKills = attackingUnitWillKillPercentage < 0.6;
                            if (attackingUnitKills)
                            {
                                defendingArmyUnitsKilled++;
                            }
                        }

                        // calculate how many attacking army units were killed
                        int attackingArmyUnitsKilled = 0;
                        for (int i = 0; i < defender.Army && attackingArmyUnitsKilled < realAttackingArmy; i++)
                        {

                            double defendingUnitWillKillPercentage = random.Next(100) / 100d;

                            // defending unit has 70% chance to kill attacking unit
                            bool defendingUnitKills = defendingUnitWillKillPercentage < 0.7;
                            if (defendingUnitKills)
                            {
                                attackingArmyUnitsKilled++;
                            }
                        }

                        defender.Army -= defendingArmyUnitsKilled;

                        realAttackingArmy -= attackingArmyUnitsKilled;
                        attacker.Army -= attackingArmyUnitsKilled;

                        if (realAttackingArmy > 0 && defender.Army == 0)
                        {
                            defender.Army -= realAttackingArmy;
                            // region was conquered
                            defender.Owner = attack.Attacker.Owner;
                            // cuz of negative units
                            defender.Army = -defender.Army;
                        }
                    }
                }
            }

            var linearizedRound = GameRound.Process(Rounds);
            Rounds.Clear();

            // deploying
            PlayDeploying(linearizedRound);

            // attacking
            PlayAttacking(linearizedRound);
            Game.Refresh();
            Game.RoundNumber++;
        }

    }

    public sealed class HotseatGameFlowHandler : GameFlowHandler
    {
        readonly IEnumerator<HumanPlayer> playersEnumerator;

        public HumanPlayer PlayerOnTurn
        {
            get { return playersEnumerator?.Current; }
        }

        public HotseatGameFlowHandler(Game game) : base(game)
        {
            var localPlayers = (from player in game.Players
                                where player.GetType() == typeof(HumanPlayer)
                                let humanPlayer = (HumanPlayer)player
                                where humanPlayer.User.UserType == UserType.LocalUser
                                      || humanPlayer.User.UserType == UserType.MyNetworkUser
                                select humanPlayer);
            playersEnumerator = localPlayers.GetEnumerator();
            NextPlayer();
        }

        /// <summary>
        /// Moves to next player.
        /// </summary>
        /// <returns>False, if player is theres no next player, true otherwise.</returns>
        public bool NextPlayer()
        {
            bool isThereNextPlayer = playersEnumerator.MoveNext();

            // theres no next local player
            if (!isThereNextPlayer) return false;

            // the player was defeated
            if (PlayerOnTurn.IsDefeated(gameState))
                return NextPlayer();

            return true;
        }

        public override void PlayRound(GameBeginningRound round)
        {
            // cannot play round if theres any other player to play
            if (NextPlayer()) throw new ArgumentOutOfRangeException();

            base.PlayRound(round);
            playersEnumerator.Reset();
        }

        public override void PlayRound()
        {
            // cannot play round if theres any other player to play
            if (NextPlayer()) throw new ArgumentOutOfRangeException();

            base.PlayRound();
            playersEnumerator.Reset();
        }
    }

    public sealed class SingleplayerGameFlowHandler : GameFlowHandler
    {
        public SingleplayerGameFlowHandler(Game game) : base(game)
        {
        }
    }

    static class PlayerExtensions
    {
        public static bool IsDefeated(this Player player, GameState state)
        {
            return player.ControlledRegions.Count == 0 && state != GameState.GameBeginning;
        }
    }

}
