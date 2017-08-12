using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GameObjectsLib;
using GameObjectsLib.GameMap;
using GameObjectsLib.GameUser;
using GameObjectsLib.NetworkCommObjects;
using ProtoBuf;

namespace GameObjectsLib.Game
{
    interface ISaveable
    {
        void Save(IGameSaver canSave);
    }

    /// <summary>
    /// Enum containing types of game any game can have.
    /// </summary>
    public enum GameType
    {
        None,
        SinglePlayer,
        MultiplayerHotseat,
        MultiplayerNetwork
    }


    /// <summary>
    /// Represents one game.
    /// </summary>
    [Serializable]
    [ProtoContract]
    [ProtoInclude(10, typeof(SingleplayerGame))]
    [ProtoInclude(11, typeof(HotseatGame))]
    [ProtoInclude(12, typeof(NetworkGame))]
    public abstract class Game : ISaveable, IRefreshable
    {
        [ProtoMember(1)]
        public int Id { get; }
        [ProtoMember(2)]
        public int RoundNumber { get; private set; }

        protected Game()
        {
        }

        /// <summary>
        /// Represents map being played in this game.
        /// </summary>
        [ProtoMember(3)]
        public Map Map { get; }

        /// <summary>
        /// Represents list of players playing this game.
        /// </summary>
        [ProtoMember(4)]
        public IList<Player> Players { get; }
        
        /// <summary>
        /// Return game type this game has.
        /// </summary>
        public abstract GameType GameType { get; }

        protected Game(int id, Map map, IList<Player> players)
        {
            Id = id;
            this.Map = map;
            this.Players = players;
        }

        /// <summary>
        /// Plays given round, calculating everything, moving this instance of
        /// the game into position after the round was played.
        /// </summary>
        /// <param name="round"></param>
        public void Play(Round round)
        {
            // TODO: debug
            // deploying
            {
                var deploying = round.Deploying;
                foreach (var deployedArmies in deploying.ArmiesDeployed)
                {
                    var region = (from item in Map.Regions
                                  where item == deployedArmies.Item1
                                  select item).First();
                    region.Army = deployedArmies.Item2;
                }
            }
            // attacking
            {
                var attacks = round.Attacking.Attacks;
                foreach (var attack in attacks)
                {
                    // TODO: real calculation according to rules
                    // get real attacker
                    var attacker = (from region in Map.Regions
                                    where region == attack.Attacker
                                    select region).First();

                    // if attacking region changed owner, cancel attack
                    if (attacker.Owner != attack.Attacker.Owner) continue;

                    // get real defender
                    var defender = (from region in Map.Regions
                                    where region == attack.Defender
                                    select region).First();
                    // situation might have changed => recalculate attacking army
                    int realAttackingArmy = Math.Min(attack.AttackingArmy, attacker.Army);
                    // if they have same owner
                    if (defender.Owner == attacker.Owner)
                    {
                        // sum armies
                        defender.Army += realAttackingArmy;
                    }
                    else
                    {
                        defender.Army -= realAttackingArmy;
                        if (defender.Army < 0) // all units were overcome and some of enemies units stayed
                        {
                            // region was conquered
                            defender.Owner = attack.Attacker.Owner;
                            // cuz of negative units
                            defender.Army = -defender.Army;
                        }
                    }
                    // in any way, attacker transfered units
                    attacker.Army -= realAttackingArmy;
                }
            }
            Refresh();
            RoundNumber++;
        }

        /// <summary>
        /// Plays given initial round, refreshing the situation of the game.
        /// </summary>
        /// <param name="round">Round to be played.</param>
        public void Play(GameBeginningRound round)
        {
            foreach (var roundSelectedRegion in round.SelectedRegions)
            {
                var realRegion = (from region in Map.Regions
                                  where region == roundSelectedRegion.Item2
                                  select region).First();
                var realPlayer = (from player in Players
                                  where player == roundSelectedRegion.Item1
                                  select player).First();

                realRegion.Owner = realPlayer;
            }

            Refresh();
            RoundNumber++;
        }

        /// <summary>
        /// Starts the game if theres no error.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// Creates an instance of new <see cref="Game"/>, validates it and returns it.
        /// </summary>
        /// <param name="id">Id of the game corresponding to Id that will be stored in the database.</param>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="map">Map of the game.</param>
        /// <param name="players">Players that will be playing the game.</param>
        /// <returns>Created instance of the game.</returns>
        public static Game Create(int id, GameType gameType, Map map, IList<Player> players)
        {
            switch (gameType)
            {
                case GameType.SinglePlayer:
                    var sp = new SingleplayerGame(id, map, players);
                    sp.Validate();
                    return sp;
                case GameType.MultiplayerHotseat:
                    var hotseatGame = new HotseatGame(id, map, players);
                    return hotseatGame;
                case GameType.MultiplayerNetwork:
                    var networkGame = new NetworkGame(id, map, players);
                    return networkGame;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Saves the game to the object based on parameter.
        /// </summary>
        /// <param name="canSave">Object which can save the current instance of the game.</param>
        public void Save(IGameSaver canSave)
        {
            Refresh();
            using (MemoryStream stream = new MemoryStream())
            {
                SerializationObjectWrapper wrapper = new SerializationObjectWrapper<Game>() {TypedValue = this};
                wrapper.Serialize(stream);
                // reset position to be able to read from it again
                stream.Position = 0;
                canSave.SaveGame(this, stream);
            }
        }

        public async Task SaveGameAsync(IGameSaver canSave)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Refreshes situation in the game.
        /// </summary>
        public void Refresh()
        {
            
            // refresh regions owner
            foreach (var region in Map.Regions)
            {
                region.Refresh();
            }
            // refresh
            foreach (var player in Players)
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
        /// Loads the game based on parameters.
        /// </summary>
        /// <typeparam name="TLoadSource">Source type.</typeparam>
        /// <param name="canLoad">Object which can load the game.</param>
        /// <param name="source">Source from which we can load the game.</param>
        /// <returns>Loaded game.</returns>
        public static Game Load<TLoadSource>(IGameLoader<TLoadSource> canLoad, TLoadSource source)
        {
            using (var stream = canLoad.LoadGame(source))
            {
                Game game = (Game)(SerializationObjectWrapper.Deserialize(stream).Value);
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
        /// After deserialization we have same objects that do not match references.
        /// Purpose of this method is to remap those references back for future
        /// graph updating to be easy..
        /// </summary>
        private void ReconstructOriginalGraph()
        {
            // TODO: IMPORTANT = check how it differs from Refresh
            var regions = Map.Regions;
            var superRegions = Map.SuperRegions;
            var players = Players;
            // reconstruct original super regions
            {
                // SuperRegion = region.SuperRegion
                foreach (Region region in regions)
                {
                    for (int j = 0; j < superRegions.Count; j++)
                    {
                        if (superRegions[j] == region.SuperRegion)
                            superRegions[j] = region.SuperRegion;
                    }
                }
            }
            // reconstruct super regions and regions
            {
                // superRegion.Region = region
                foreach (Region region in regions)
                {
                    foreach (var superRegion in superRegions)
                    {
                        var superRegionRegions = superRegion.Regions;
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
                    var neighbours = regions[i].NeighbourRegions;
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
                foreach (var player in Players)
                {
                    var controlledRegions = player.ControlledRegions;
                    foreach (var controlledRegion in controlledRegions)
                    {
                        controlledRegion.Owner = player;
                    }
                }
                // other way
                foreach (var player in players)
                {
                    foreach (Region region in Map.Regions)
                    {
                        if (region.Owner == player) region.Owner = player;
                    }
                }
            }

        }
    }
    
}