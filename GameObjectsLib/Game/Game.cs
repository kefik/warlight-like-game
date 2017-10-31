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
        public IList<Round> AllRounds { get; } = new List<Round>();

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

        /// <summary>
        ///     Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }
        
        protected Game()
        {
        }

        protected Game(int id, Map map, IList<Player> players, bool isFogOfWar)
        {
            Id = id;
            Map = map;
            Players = players;
            IsFogOfWar = isFogOfWar;
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