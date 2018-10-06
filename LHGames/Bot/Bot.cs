using System;
using System.Collections.Generic;
using LHGames.Helper;

namespace LHGames.Bot
{
    internal class Bot
    {
        enum ETATS { COLLECTER, DEPLACER, ATTAQUER, DEFENDRE, UPGRADE, VOLER, RECHERCHER };
        int presentState = (int)ETATS.COLLECTER;
        int previousState;
        static IPlayer PlayerInfo { get; set; }
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
            foreach(Tile t in map.GetVisibleTiles())
            {
                if(t.TileType == TileContent.Player)
                {
                    if(PlayerInfo.Position.X + 1 == t.Position.X || PlayerInfo.Position.X - 1 == t.Position.X ||
                       PlayerInfo.Position.Y + 1 == t.Position.Y || PlayerInfo.Position.Y - 1 == t.Position.Y)
                    {
                        presentState = (int)ETATS.DEFENDRE; // si l'ennemi est à côté de nous
                    }
                    else // si l'ennemi n'est pas à côté
                    {
                        presentState = (int)ETATS.ATTAQUER; 
                    }
                }
                else if (t.TileType == TileContent.Resource)
                {
                    presentState = (int)ETATS.COLLECTER;
                }
                else // Si on ne voit rien de pertinent
                {
                    presentState = (int)ETATS.RECHERCHER;
                }
            }

            if (map.GetTileAt(PlayerInfo.Position.X + _currentDirection, PlayerInfo.Position.Y) == TileContent.Wall)
            {
                _currentDirection *= -1;
            }

            switch (presentState)
            {
                case (int)ETATS.COLLECTER:
                    //collection.Collecter();
                    break;
                case (int)ETATS.DEPLACER:
                    //movement.Deplacer();
                    break;
                case (int)ETATS.ATTAQUER:
                    //actions.Attaquer();
                    break;
                case (int)ETATS.DEFENDRE:
                    //actions.Defendre();
                    break;
                case (int)ETATS.UPGRADE:
                    //actions.Upgrade();
                    break;
                case (int)ETATS.VOLER:
                    //actions.Steal();
                    break;
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
            /// Used by the other classes to specify a point where we need to move
            /// ie: the player wan't to move to a point positioned 1 right, 3 up, MoveTo(new Point(1, -3));
            /// </summary>
            /// <param name="point">The point where the player wants to end up</param>
            public void MoveTo(Point point)
            {
                List<Point> path = FindPath(point);
            }

            /// <summary>
            /// Will create a table containing multiple MoveActions
            /// Every MoveActions will be a direction point, 
            /// ie: [1, 0], [1, 0], [0,-1] would represent the (right, right, up) path
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            private List<Point> FindPath(Point point)
            {
                List<Point> path = new List<Point> { }; 



                return path;
            }

            /// <summary>
            /// When called, the player will move in a specific direction
            /// Input must be between [-1, -1] and [1, 1], and can only have 1 parameter != 0
            /// </summary>
            /// <param name="point">Move in the x axis. Left = [-1, 0], Right = [1, 0]
            ///                     Move in the y axis. Top = [0, -1], Down = [0, 1] </param>
            private void Move(Point point)
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
            private MovementActions movement;

            public MovementActions Movement
            {
                get => movement;
                set
                {
                    movement = value;
                }
            }

            public PlayerActions()
            {
                Movement = new MovementActions();
            }

            //Find the distance to nearest enemy
            public void MeleeAttack(IEnumerable<IPlayer> visiblePlayers)
            {
                Point target = new Point(0, 0);
                double distance = int.MaxValue;
                foreach(var p in visiblePlayers)
                {
                    Point temp = p.Position;
                    double distanceTemp = Math.Sqrt(Math.Pow((PlayerInfo.Position.X - temp.X), 2) + Math.Pow((PlayerInfo.Position.Y - temp.Y), 2));
                    if (distanceTemp < distance)
                    {
                        target = temp;
                        distance = distanceTemp;
                    }
                }
                if (distance > 1)
                {
                    //movement.Deplacer(target);
                }
                else
                {
                    Point direction = new Point(target.X - PlayerInfo.Position.X, target.Y - PlayerInfo.Position.Y);
                    AIHelper.CreateMeleeAttackAction(direction);
                }
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