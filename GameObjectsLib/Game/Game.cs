namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GameMap;
    using NetworkCommObjects;
    using ProtoBuf;

    internal interface ISaveable<out T>
    {
        void Save(IGameSaver<T> canSave);
    }

    /// <summary>
    ///     Represents one game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    [ProtoInclude(10, typeof(SingleplayerGame))]
    [ProtoInclude(11, typeof(HotseatGame))]
    [ProtoInclude(12, typeof(NetworkGame))]
    public abstract class Game : ISaveable<Game>, IRefreshable
    {
        [ProtoMember(1)]
        public int Id { get; }

        [ProtoMember(2)]
        public int RoundNumber { get; set; }

        /// <summary>
        /// True, if this game has a fog of war option.
        /// </summary>
        [ProtoMember(3)]
        public bool IsFogOfWar { get; }

        protected Game()
        {
        }

        /// <summary>
        ///     Represents map being played in this game.
        /// </summary>
        [ProtoMember(4)]
        public Map Map { get; }

        /// <summary>
        ///     Represents list of players playing this game.
        /// </summary>
        [ProtoMember(5)]
        public IList<Player> Players { get; }

        /// <summary>
        ///     Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }

        protected Game(int id, Map map, IList<Player> players, bool isFogOfWar)
        {
            Id = id;
            Map = map;
            Players = players;
            IsFogOfWar = isFogOfWar;
        }

        /// <summary>
        ///     Plays given round, calculating everything, moving this instance of
        ///     the game into position after the round was played.
        /// </summary>
        /// <param name="gameRound"></param>
        public void Play(GameRound gameRound)
        {
            void PlayDeploying()
            {
                Deploying deploying = gameRound.Deploying;
                foreach (var deployedArmies in deploying.ArmiesDeployed)
                {
                    Region region = (from item in Map.Regions
                                     where item == deployedArmies.Region
                                     select item).First();
                    region.Army = deployedArmies.Army;
                }
            }

            void PlayAttacking()
            {
                var attacks = gameRound.Attacking.Attacks;
                foreach (Attack attack in attacks)
                {
                    // TODO: real calculation according to rules
                    // get real attacker
                    Region attacker = (from region in Map.Regions
                                       where region == attack.Attacker
                                       select region).First();

                    // if attacking region changed owner, cancel attack
                    if (attacker.Owner != attack.Attacker.Owner)
                    {
                        continue;
                    }

                    // get real defender
                    Region defender = (from region in Map.Regions
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
                        for (int i = 0; (i < realAttackingArmy) && (defendingArmyUnitsKilled < defender.Army); i++)
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
                        for (int i = 0; (i < defender.Army) && (attackingArmyUnitsKilled < realAttackingArmy); i++)
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
                        attacker.Army -= attackingArmyUnitsKilled;
                        // what remained from attacking army
                        realAttackingArmy -= attackingArmyUnitsKilled;

                        // not whole attacking army was destroyed and defending army is destroyed
                        if ((realAttackingArmy > 0) && (defender.Army == 0))
                        {
                            attacker.Army -= realAttackingArmy;
                            defender.Army -= realAttackingArmy;
                            // region was conquered
                            defender.Owner = attack.Attacker.Owner;
                            // cuz of negative units
                            defender.Army = -defender.Army;
                        }
                    }
                }
            }

            // deploying
            PlayDeploying();

            // attacking
            PlayAttacking();
            Refresh();
            RoundNumber++;
        }

        /// <summary>
        ///     Plays given initial round, refreshing the situation of the game.
        /// </summary>
        /// <param name="round">Round to be played.</param>
        public void Play(GameBeginningRound round)
        {
            foreach (Tuple<Player, Region> roundSelectedRegion in round.SelectedRegions)
            {
                Region realRegion = (from region in Map.Regions
                                     where region == roundSelectedRegion.Item2
                                     select region).First();
                Player realPlayer = (from player in Players
                                     where player == roundSelectedRegion.Item1
                                     select player).First();

                realRegion.Owner = realPlayer;
            }

            Refresh();
            RoundNumber++;
        }

        /// <summary>
        ///     Starts the game if theres no error.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        ///     Creates an instance of new <see cref="Game" />, validates it and returns it.
        /// </summary>
        /// <param name="id">Id of the game corresponding to Id that will be stored in the database.</param>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="map">Map of the game.</param>
        /// <param name="players">Players that will be playing the game.</param>
        /// <param name="fogOfWar"></param>
        /// <returns>Created instance of the game.</returns>
        public static Game Create(int id, GameType gameType, Map map, IList<Player> players, bool fogOfWar)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                    SingleplayerGame sp = new SingleplayerGame(id, map, players, fogOfWar);
                    sp.Validate();
                    return sp;
                case GameType.MultiplayerHotseat:
                    HotseatGame hotseatGame = new HotseatGame(id, map, players, fogOfWar);
                    return hotseatGame;
                case GameType.MultiplayerNetwork:
                    NetworkGame networkGame = new NetworkGame(id, map, players, fogOfWar);
                    return networkGame;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Saves the game to the object based on parameter.
        /// </summary>
        /// <param name="canSave">Object which can save the current instance of the game.</param>
        public void Save(IGameSaver<Game> canSave)
        {
            Refresh();
            using (MemoryStream stream = new MemoryStream())
            {
                SerializationObjectWrapper wrapper = new SerializationObjectWrapper<Game> {TypedValue = this};
                wrapper.Serialize(stream);
                // reset position to be able to read from it again
                stream.Position = 0;
                canSave.SaveGame(this, stream);
            }
        }


        public byte[] GetBytes()
        {
            SerializationObjectWrapper wrapper
                = new SerializationObjectWrapper<Game>
                {
                    TypedValue = this
                };
            using (MemoryStream ms = new MemoryStream())
            {
                wrapper.Serialize(ms);
                ms.Position = 0;
                return ms.GetBuffer();
            }
        }

        public MemoryStream GetStreamForSerializedGame()
        {
            SerializationObjectWrapper wrapper
                = new SerializationObjectWrapper<Game>
                {
                    TypedValue = this
                };
            MemoryStream ms = new MemoryStream();
            wrapper.Serialize(ms);

            ms.Position = 0;

            return ms;
        }

        /// <summary>
        ///     Refreshes situation in the game.
        /// </summary>
        public void Refresh()
        {
            // refresh regions owner
            foreach (Region region in Map.Regions)
            {
                region.Refresh();
            }
            // refresh
            foreach (Player player in Players)
            {
                player.Refresh();
            }
            // refresh super regions
            foreach (SuperRegion superRegion in Map.SuperRegions)
            {
                superRegion.Refresh();
            }
        }

        /// <summary>
        ///     Loads the game based on parameters.
        /// </summary>
        /// <typeparam name="TLoadSource">Source type.</typeparam>
        /// <param name="canLoad">Object which can load the game.</param>
        /// <param name="source">Source from which we can load the game.</param>
        /// <returns>Loaded game.</returns>
        public static Game Load<TLoadSource>(IGameLoader<TLoadSource> canLoad, TLoadSource source)
        {
            using (Stream stream = canLoad.LoadGame(source))
            {
                Game game = (Game) SerializationObjectWrapper.Deserialize(stream).Value;
                game.ReconstructOriginalGraph();
                game.Refresh();
                return game;
            }
        }

        public static async Task<Game> LoadAsync<TLoadSource>(IGameLoader<TLoadSource> canLoad, TLoadSource source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     After deserialization we have same objects that do not match references.
        ///     Purpose of this method is to remap those references back for future
        ///     graph updating to be easy..
        /// </summary>
        private void ReconstructOriginalGraph()
        {
            // TODO: IMPORTANT = check how it differs from Refresh
            IList<Region> regions = Map.Regions;
            IList<SuperRegion> superRegions = Map.SuperRegions;
            IList<Player> players = Players;
            // reconstruct original super regions
            {
                // SuperRegion = region.SuperRegion
                foreach (Region region in regions)
                {
                    for (int j = 0; j < superRegions.Count; j++)
                    {
                        if (superRegions[j] == region.SuperRegion)
                        {
                            superRegions[j] = region.SuperRegion;
                        }
                    }
                }
            }
            // reconstruct super regions and regions
            {
                // superRegion.Region = region
                foreach (Region region in regions)
                {
                    foreach (SuperRegion superRegion in superRegions)
                    {
                        IList<Region> superRegionRegions = superRegion.Regions;
                        for (int j = 0; j < superRegionRegions.Count; j++)
                        {
                            if (superRegionRegions[j] == region)
                            {
                                superRegionRegions[j] = region;
                            }
                        }
                    }
                }

                // region = superRegion.Region
                // should be fine
            }


            // reconstruct neighbour regions
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    IList<Region> neighbours = regions[i].NeighbourRegions;
                    foreach (Region neighbour in neighbours)
                    {
                        for (int j = 0; j < regions.Count; j++)
                        {
                            if (regions[j] == neighbour)
                            {
                                regions[j] = neighbour;
                            }
                        }
                    }
                }
            }
            // reconstruct owner
            {
                foreach (Player player in Players)
                {
                    IList<Region> controlledRegions = player.ControlledRegions;
                    foreach (Region controlledRegion in controlledRegions)
                    {
                        controlledRegion.Owner = player;
                    }
                }
                // other way
                foreach (Player player in players)
                {
                    foreach (Region region in Map.Regions)
                    {
                        if (region.Owner == player)
                        {
                            region.Owner = player;
                        }
                    }
                }
            }
        }
    }
}