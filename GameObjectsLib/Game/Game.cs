﻿namespace GameObjectsLib.Game
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GameAi.Data.Restrictions;
    using GameMap;
    using GameRecording;
    using GameRestrictions;
    using NetworkCommObjects;
    using Players;
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
    [ProtoInclude(13, typeof(SimulatorGame))]
    public abstract class Game : ISaveable<Game>, IRefreshable
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// Represents number of the current round.
        /// </summary>
        public int RoundNumber
        {
            get { return AllRounds.Count; }
        }

        /// <summary>
        /// Represents list of all rounds of the game.
        /// </summary>
        [ProtoMember(2)]
        public IList<ILinearizedRound> AllRounds { get; } = new List<ILinearizedRound>();

        /// <summary>
        /// True, if this game has a fog of war option.
        /// </summary>
        [ProtoMember(3)]
        public bool IsFogOfWar { get; }

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

        [ProtoMember(6)]
        public GameObjectsRestrictions ObjectsRestrictions { get; }

        /// <summary>
        ///     Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }

        protected Game()
        {
        }

        protected Game(Map map, IList<Player> players, bool isFogOfWar, GameObjectsRestrictions objectsRestrictions)
        {
            Map = map;
            Players = players;
            IsFogOfWar = isFogOfWar;

            if (objectsRestrictions == null)
            {
                // initialize with all regions
                var gameBeginningRestrictions = new List<GameObjectsBeginningRestriction>();
                var regions = map.Regions;
                foreach (Player player in players)
                {
                    gameBeginningRestrictions.Add(new GameObjectsBeginningRestriction()
                    {
                        Player = player,
                        RegionsPlayersCanChoose = regions,
                        RegionsToChooseCount = 2
                    });
                }
                ObjectsRestrictions = new GameObjectsRestrictions()
                {
                    GameBeginningRestrictions = gameBeginningRestrictions
                };
            }
            else
            {
                ObjectsRestrictions = objectsRestrictions;
            }
        }

        /// <summary>
        ///     Starts the game if theres no error.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        ///     Saves the game to the object based on parameter.
        /// </summary>
        /// <param name="canSave">Object which can save the current instance of the game.</param>
        public void Save(IGameSaver<Game> canSave)
        {
            Refresh();
            canSave.SaveGame(this);
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
            // refresh super regions
            foreach (SuperRegion superRegion in Map.SuperRegions)
            {
                superRegion.Refresh();
            }
        }

        /// <summary>
        /// Deep copies <see cref="Game"/> instance.
        /// </summary>
        /// <returns></returns>
        public Game DeepCopy()
        {
            Game game;
            using (var ms = GetStreamForSerializedGame())
            {
                // deep copy the game so its independent on current bot playing
                game = (Game)SerializationObjectWrapper.Deserialize(ms).Value;
                game.ReconstructOriginalGraph();
            }
            game.Refresh();

            return game;
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
            byte[] serializedGame = canLoad.LoadGame(source);
            Game game = (Game)SerializationObjectWrapper.Deserialize(serializedGame).Value;
            game.ReconstructOriginalGraph();
            game.Refresh();
            return game;
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
        internal void ReconstructOriginalGraph()
        {
            // SuperRegions and its regions
            foreach (var superRegion in Map.SuperRegions)
            {
                foreach (var superRegionRegion in superRegion.Regions)
                {
                    // map superRegion according to main
                    superRegionRegion.SuperRegion = superRegion;

                    // map Map.Regions = superRegion.Regions
                    for (int i = 0; i < Map.Regions.Count; i++)
                    {
                        if (Map.Regions[i] == superRegionRegion)
                        {
                            Map.Regions[i] = superRegionRegion;
                            break;
                        }
                    }
                }
            }

            // remap Regions and neighbour regions
            foreach (var region in Map.Regions)
            {
                // iterate through neighbours and connect them to Map.Region
                for (int i = 0; i < region.NeighbourRegions.Count; i++)
                {
                    var realRegion = Map.Regions.First(x => x == region.NeighbourRegions[i]);
                    region.NeighbourRegions[i] = realRegion;
                }
            }

            // remap player owned regions
            foreach (var player in Players)
            {
                // remap region to real region
                for (int i = 0; i < player.ControlledRegions.Count; i++)
                {
                    var realRegion = Map.Regions.First(x => x == player.ControlledRegions[i]);
                    realRegion.Owner = player;
                    player.ControlledRegionsInternal[i] = realRegion;
                }
            }

            // remap rounds
            foreach (var linearizedRound in AllRounds)
            {
                switch (linearizedRound)
                {
                    case LinearizedGameRound round:
                        var deploying = round.Deploying;
                        foreach (Deployment deployment in deploying.ArmiesDeployed)
                        {
                            deployment.Region = Map.Regions.First(x => x == deployment.Region);
                            deployment.DeployingPlayer = Players.First(x => x == deployment.DeployingPlayer);
                        }

                        var attacking = round.Attacking;
                        foreach (Attack attack in attacking.Attacks)
                        {
                            attack.Attacker = Map.Regions.First(x => x == attack.Attacker);
                            attack.Defender = Map.Regions.First(x => x == attack.Defender);
                            attack.AttackingPlayer = Players.First(x => x == attack.AttackingPlayer);

                            var mapChange = attack.PostAttackMapChange;
                            if (mapChange?.DefendingRegionOwner != null)
                            {
                                mapChange.DefendingRegionOwner = Players.First(x => x == mapChange.DefendingRegionOwner);
                            }
                        }
                        break;
                    case LinearizedGameBeginningRound round:
                        var selectedRegions = round.SelectedRegions;
                        foreach (Seize selectedRegion in selectedRegions)
                        {
                            selectedRegion.Region = Map.Regions.First(x => x == selectedRegion.Region);
                            selectedRegion.SeizingPlayer = Players.First(x => x == selectedRegion.SeizingPlayer);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Reports whether the game is finished. Game is finished if its not beginning of the game and exactly one player controls some regions.
        /// </summary>
        /// <returns></returns>
        public bool IsFinished()
        {
            // is round number > 0 and number of players that have regions == 1
            return Players.Count(x => x.ControlledRegions.Count > 0) == 1 && RoundNumber > 0;
        }
    }
}