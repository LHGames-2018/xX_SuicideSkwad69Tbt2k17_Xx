using System;
using System.Collections.Generic;
using LHGames.Helper;

namespace LHGames.Bot
{
    internal class Bot
    {
        internal IPlayer PlayerInfo { get; set; }
        private int _currentDirection = 1;

        internal Bot() { }

        /// <summary>
        /// Gets called before ExecuteTurn. This is where you get your bot's state.
        /// </summary>
        /// <param name="playerInfo">Your bot's current state.</param>
        internal void BeforeTurn(IPlayer playerInfo)
        {
            PlayerInfo = playerInfo;
        }

        /// <summary>
        /// Implement your bot here.
        /// </summary>
        /// <param name="map">The gamemap.</param>
        /// <param name="visiblePlayers">Players that are visible to your bot.</param>
        /// <returns>The action you wish to execute.</returns>
        internal string ExecuteTurn(Map map, IEnumerable<IPlayer> visiblePlayers)
        {
            MovementActions movement = new MovementActions();
            PlayerActions actions = new PlayerActions();
            CollectActions collection = new CollectActions(map);

            // TODO: Implement your AI here.
            if (map.GetTileAt(PlayerInfo.Position.X + _currentDirection, PlayerInfo.Position.Y) == TileContent.Wall)
            {
                _currentDirection *= -1;
            }

            var data = StorageHelper.Read<TestClass>("Test");
            Console.WriteLine(data?.Test);
            return AIHelper.CreateMoveAction(new Point(_currentDirection, 0));
        }

        /// <summary>
        /// Gets called after ExecuteTurn.
        /// </summary>
        internal void AfterTurn()
        {
        }

        /// <summary>
        /// Class used to define the path to use
        /// </summary>
        class MovementActions
        {
            public MovementActions()
            {

            }

            public void Deplacer(int x, int y)
            {

            }
        }

        /// <summary>
        /// Class that deals with the player actions
        /// </summary>
        class PlayerActions
        {
            public PlayerActions()
            {

            }
        }

        /// <summary>
        /// Class that deals with the player collect actions
        /// </summary>
        class CollectActions
        {
            public CollectActions(Map m)
            {
                Map map = m;
                foreach (Tile t in m.VisibleDistance) {
                    
                }

            }
        }
    }
}

class TestClass
{
    public string Test { get; set; }
}