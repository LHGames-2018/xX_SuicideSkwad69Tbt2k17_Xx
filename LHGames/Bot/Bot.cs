using System;
using System.Collections.Generic;
using LHGames.Helper;
using System.Linq;

namespace LHGames.Bot
{
    internal class Bot
    {
        enum ETATS { COLLECTER, ATTAQUER, DEFENDRE, UPGRADE, VOLER, RECHERCHER, RETOURNER_MAISON };
        int presentState = (int)ETATS.COLLECTER;
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
            PlayerActions actions = new PlayerActions(map);
            Point direction = new Point(0, 0);

            // Scanning the map
            foreach(Tile t in map.GetVisibleTiles())
            {
                //if(t.TileType == TileContent.Player) // 1e priorite : attaquer et defendre
                //{
                //    if(PlayerInfo.Position.X + 1 == t.Position.X || PlayerInfo.Position.X - 1 == t.Position.X ||
                //       PlayerInfo.Position.Y + 1 == t.Position.Y || PlayerInfo.Position.Y - 1 == t.Position.Y)
                //    {
                //        presentState = (int)ETATS.DEFENDRE; // si l'ennemi est à côté de nous
                //        direction = new Point(t.Position.X - PlayerInfo.Position.X, t.Position.Y - PlayerInfo.Position.Y);
                //    }
                //    else // si l'ennemi n'est pas à côté
                //    {
                //        presentState = (int)ETATS.ATTAQUER; 
                //    }
                //}
                if (t.TileType == TileContent.Resource) // 2e priorite : collecter des ressources
                {
                    presentState = (int)ETATS.COLLECTER;
                }
                else // Si on ne voit rien de pertinent // derniere priorite : rechercher des ressources
                {
                    presentState = (int)ETATS.RECHERCHER;
                }
            }
            //if(PlayerInfo.CarriedResources >= 1000) // initial capacity is 1000
            //{
            //    presentState = (int)ETATS.RETOURNER_MAISON;
            //}
            //if(PlayerInfo.TotalResources > UpgradeType.AttackPower)
            //{
            //    presentState = (int)ETATS.UPGRADE;
            //}

            switch (presentState)
            {
                case (int)ETATS.COLLECTER:
                    CollectActions.Collect(map);
                    break;
                case (int)ETATS.ATTAQUER:
                    //actions.Attaquer();
                    break;
                case (int)ETATS.DEFENDRE:
                    //actions.Defendre(direction);
                    break;
                case (int)ETATS.UPGRADE:
                    //actions.Upgrade();
                    break;
                case (int)ETATS.VOLER:
                    //actions.Steal();
                    break;
                case (int)ETATS.RECHERCHER:
                    //actions.Rechercher(); // plus rien sur la map visible
                    break;
                case (int)ETATS.RETOURNER_MAISON:
                    //CollectActions.RetournerMaison(map);
                    break;
            }

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
        public static class MovementActions
        {
            //public MovementActions(Map map)
            //{
            //    MapInstance = map;
            //}

            //Map MapInstance { get; set; }

            /// <summary>
            /// Used by the other classes to specify a point where we need to move
            /// ie: the player wan't to move to a point positioned 1 right, 3 up, MoveTo(new Point(player.X + 1, Player.Y - 3));
            /// </summary>
            /// <param name="point">The point where the player wants to end up</param>
            public static void MoveTo(Map map, Point point)
            {
                //List<Point> path = FindPath(map, point);
                FindEasyPath(map, point);
            }

            private static void FindEasyPath(Map map, Point point)
            {
                bool moveLeft = false,
                     moveRight = false,
                     moveUp = false,
                     moveDown = false;

                int nbMovesX = point.X - PlayerInfo.Position.X;
                int nbMovesY = point.Y - PlayerInfo.Position.Y;

                if(nbMovesX > 0)
                {
                    moveRight = true;
                }
                else if(nbMovesX < 0)
                {
                    moveLeft = true;
                }
                if(nbMovesY > 0)
                {
                    moveDown = true;
                }
                else if(nbMovesY < 0)
                {
                    moveUp = true;
                }

                Random random = new Random(DateTime.Now.Millisecond);
                if(random.Next() % 2 == 0)
                {
                    if (moveLeft)
                    {
                        AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    else if(moveRight)
                    {
                        AIHelper.CreateMoveAction(new Point(1, 0));
                    }
                    else if(moveUp)
                    {
                        AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                    else if (moveDown)
                    {
                        AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                }
                else
                {
                    if (moveUp)
                    {
                        AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                    else if (moveDown)
                    {
                        AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                    else if (moveLeft)
                    {
                        AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    else if (moveRight)
                    {
                        AIHelper.CreateMoveAction(new Point(1, 0));
                    }
                }
            }

            /// <summary>
            /// Will create a table containing multiple MoveActions
            /// Every MoveActions will be a direction point, 
            /// ie: [1, 0], [1, 0], [0,-1] would represent the (right, right, up) path
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            private static List<Point> FindPath(Map map, Point point)
            {
                // this list will contain all the moves to make to ge to the desired point
                List<Point> path = new List<Point> { };
                List<Point> emptyPoints = new List<Point> { }; // this list contains all the available points for moves
                Point tempPosition;

                foreach (Tile t in map.GetVisibleTiles())
                {
                    if (t.TileType != TileContent.Empty)
                    {
                        emptyPoints.Add(t.Position);
                    }
                }

                //A*
                double flyingDistance = Math.Sqrt(Math.Pow((PlayerInfo.Position.X - point.X), 2) + Math.Pow((PlayerInfo.Position.Y - point.Y), 2));
                int tileUnitsDistance = (PlayerInfo.Position.X - point.X) + (PlayerInfo.Position.Y - point.Y);

                do
                {
                    tempPosition = new Point();
                } while (tempPosition != point);

                return path;
            }

            private static void AStarMoveUp()
            {

            }

            private static void AStarMoveDown()
            {

            }
            private static void AStarMoveLeft()
            {

            }
            private static void AStarMoveRight()
            {

            }

            /// <summary>
            /// When called, the player will move in a specific direction
            /// Input must be between [-1, -1] and [1, 1], and can only have 1 parameter != 0
            /// </summary>
            /// <param name="point">Move in the x axis. Left = [-1, 0], Right = [1, 0]
            ///                     Move in the y axis. Top = [0, -1], Down = [0, 1] </param>
            private static void Move(Point point)
            {
                if (point.X != 0 ^ point.Y != 0)
                {
                    if (point.X != 0) // move in x axis
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
            private Map gameMap;

            public Map GameMap
            {
                get { return gameMap; }
                set { gameMap = value; }
            }


            public PlayerActions(Map map)
            {           
                GameMap = map;
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
                    Defend(direction);
                }
            }

            /// <summary>
            /// Attack in the specified direction
            /// </summary>
            /// <param name="direction"></param>
            public void Attack(Point direction)
            {
                AIHelper.CreateMeleeAttackAction(direction);

            }

            /// <summary>
            /// Defend from enemies
            /// </summary>
            /// <param name="direction"></param>
            public void Defend(Point direction = null)
            {
                double criticalHp = PlayerInfo.MaxHealth * 0.3;
                if (PlayerInfo.Health <= criticalHp)
                {
                    if (PlayerInfo.CarriedItems.Count(x => x == PurchasableItem.HealthPotion) > 0)
                    {
                        AIHelper.CreateHealAction();
                    }
                }
                else
                {
                    Attack(direction);
                }
            }

            /// <summary>
            /// Search a random position, if it's a wall, break it. Else move to the direction
            /// </summary>
            public void SearchNewPosition()
            {
                Random generator = new Random();
                int negX = 1;
                int negY = 1;
                if (generator.Next(1) > 0)
                {
                    negX *= -1;
                }
                if (generator.Next(1) > 0)
                {
                    negY *= -1;
                }
                Point direction = new Point(negX * generator.Next(1), negY * generator.Next(1));
                if (GameMap.GetTileAt(direction.X, direction.Y) == TileContent.Wall)
                {
                    if (GameMap.WallsAreBreakable)
                    {
                        Attack(direction);
                    }
                    else
                    {
                        SearchNewPosition();
                    }
                }
                else
                {
                    //Movement.Deplacer(direction);
                }
            }
        }

        /// <summary>
        /// Class that deals with the player collect actions
        /// </summary>
        public static class CollectActions
        {
            private Map gameMap;

            public Map GameMap
            {
                get { return gameMap; }
                set { gameMap = value; }
            }


            public CollectActions(Map m)
            {
                GameMap = m;
            }

            /// <summary>
            /// Move to rock and if close enough, collect
            /// </summary>
            public void MoveToRock()
            {
                Point position = new Point(2000, 2000);
                if (PlayerInfo.CarriedResources == PlayerInfo.CarryingCapacity)
                {
                    MovementActions.MoveTo(GameMap, PlayerInfo.HouseLocation);
                }
                else
                {
                    foreach (Tile t in GameMap.GetVisibleTiles())
                    {
                        if (t.TileType == TileContent.Resource)
                        {
                            if (Math.Pow(t.Position.X, 2) + Math.Pow(t.Position.Y, 2) < Math.Pow(position.X, 2) + Math.Pow(position.Y, 2))
                            {
                                position = new Point(t.Position.X, t.Position.Y);
                            }
                        }
                    }
                    double distance = Math.Sqrt(Math.Pow((position.X - PlayerInfo.Position.X), 2) + (Math.Pow((position.Y - PlayerInfo.Position.Y), 2)));
                    if (distance > 1)
                    {
                        MovementActions.MoveTo(GameMap, position);
                    }
                    else
                    {
                        AIHelper.CreateCollectAction(position - PlayerInfo.Position);
                    }
                }
            }
        }
    }
}

class TestClass
{
    public string Test { get; set; }
}