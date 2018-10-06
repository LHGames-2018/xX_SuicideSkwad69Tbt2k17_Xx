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
            CollectActions collection = new CollectActions();

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

            /// <summary>
            /// When called, the player will move in a specific direction
            /// Input must be between [-1, -1] and [1, 1], and can only have 1 parameter != 0
            /// </summary>
            /// <param name="point">Move in the x axis. Left = [-1, 0], Right = [1, 0]
            ///                     Move in the y axis. Top = [0, -1], Down = [0, 1] </param>
            public void Deplacer(Point point)
            {
                if(point.X != 0 ^ point.Y != 0)
                {
                    if(point.X != 0) // move in x axis
                    {
                        AIHelper.CreateMoveAction(new Point(point.X, 0));
                    }
                    else // move in y axis
                    {
                        AIHelper.CreateMoveAction(new Point(0, point.Y));
                    }
                }
                else // Called if the user sent inconsistent entrie values
                {
                    AIHelper.CreateMoveAction(new Point(0, 0)); // Won't move
                }
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
            public CollectActions()
            {

            }
        }
    }
}

class TestClass
{
    public string Test { get; set; }
}