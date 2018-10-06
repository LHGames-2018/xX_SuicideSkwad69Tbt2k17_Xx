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
                //else // Si on ne voit rien de pertinent // derniere priorite : rechercher des ressources
                //{
                //    presentState = (int)ETATS.RECHERCHER;
                //}
            }
            //if(PlayerInfo.CarriedResources >= 1000) // initial capacity is 1000
            //{
            //    presentState = (int)ETATS.RETOURNER_MAISON;
            //}
            int upgrade = (int)UpgradeType.AttackPower;
            if (PlayerInfo.TotalResources > 10000)
            {
                presentState = (int)ETATS.UPGRADE;
            }

            string action = "";

            switch (presentState)
            {
                case (int)ETATS.COLLECTER:
                    action = CollectActions.MoveToRock(map);
                    break;
                case (int)ETATS.ATTAQUER:
                    //actions.Attaquer();
                    break;
                case (int)ETATS.DEFENDRE:
                    //actions.Defendre(direction);
                    break;
                case (int)ETATS.UPGRADE:
                    action = actions.Upgrade(upgrade);
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

            //if (map.GetTileAt(PlayerInfo.Position.X + _currentDirection, PlayerInfo.Position.Y) == TileContent.Wall)
            //{
            //    _currentDirection *= -1;
            //}

            //var data = StorageHelper.Read<TestClass>("Test");
            //Console.WriteLine(data?.Test);
            //return AIHelper.CreateMoveAction(new Point(_currentDirection, 0));
            return action;
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
            /// <summary>
            /// Used by the other classes to specify a point where we need to move
            /// ie: the player wan't to move to a point positioned 1 right & 3 up, point should be [1, -3]
            /// </summary>
            /// <param name="point"></param>
            public static string MoveTo(Map map, Point point)
            {
                return FindEasyPath(map, point);
            }

            private static string FindEasyPath(Map map, Point point)
            {
                bool moveLeft = false,
                     moveRight = false,
                     moveUp = false,
                     moveDown = false;

                if (point.X > 0)
                {
                    if(map.GetTileAt(PlayerInfo.Position.X + 1, PlayerInfo.Position.Y) != TileContent.Lava)
                    {
                        moveRight = true;
                    }
                    else
                    {
                        if(map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y - 1) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(0, -1));
                        }
                        else if(map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y + 1) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(0, 1));
                        }
                    }
                }
                else if (point.X < 0)
                {
                    if (map.GetTileAt(PlayerInfo.Position.X - 1, PlayerInfo.Position.Y) != TileContent.Lava)
                    {
                        moveLeft = true;
                    }
                    else
                    {
                        if (map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y - 1) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(0, -1));
                        }
                        else if (map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y + 1) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(0, 1));
                        }
                    }
                }

                if (point.Y > 0)
                {
                    if (map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y + 1) != TileContent.Lava)
                    {
                        moveDown = true;
                    }
                    else
                    {
                        if (map.GetTileAt(PlayerInfo.Position.X - 1, PlayerInfo.Position.Y) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(-1, 0));
                        }
                        else if (map.GetTileAt(PlayerInfo.Position.X + 1, PlayerInfo.Position.Y) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(1, 0));
                        }
                    }
                }
                else if (point.Y < 0)
                {
                    if (map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y - 1) != TileContent.Lava)
                    {
                        moveUp = true;
                    }
                    else
                    {
                        if (map.GetTileAt(PlayerInfo.Position.X - 1, PlayerInfo.Position.Y) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(-1, 0));
                        }
                        else if (map.GetTileAt(PlayerInfo.Position.X + 1, PlayerInfo.Position.Y) != TileContent.Lava)
                        {
                            return AIHelper.CreateMoveAction(new Point(1, 0));
                        }
                    }
                }

                Random random = new Random(DateTime.Now.Millisecond);
                int nbRandom = random.Next() % 4;
                if (nbRandom == 0)
                {
                    if (moveLeft)
                    {
                        return AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    else if (moveRight)
                    {
                        return AIHelper.CreateMoveAction(new Point(1, 0));
                    }
                    else if (moveUp)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                    else
                    {
                        return AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                }
                else if(nbRandom == 1)
                {
                    if(moveDown)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                    else if (moveLeft)
                    {
                        return AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    else if (moveRight)
                    {
                        return AIHelper.CreateMoveAction(new Point(1, 0));
                    }
                    else
                    {
                        return AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                }
                else if (nbRandom == 2)
                {
                    if(moveUp)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                    else if (moveDown)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                    else if (moveLeft)
                    {
                        return AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    else
                    {
                        return AIHelper.CreateMoveAction(new Point(1, 0));
                    } 
                }
                else
                {
                    if(moveRight)
                    {
                        return AIHelper.CreateMoveAction(new Point(1, 0));
                    }
                    else if (moveUp)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, -1));
                    }
                    else if (moveDown)
                    {
                        return AIHelper.CreateMoveAction(new Point(0, 1));
                    }
                    else
                    {
                        return AIHelper.CreateMoveAction(new Point(-1, 0));
                    }
                    
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

            public string Upgrade(int upgradeType)
            {
                if (PlayerInfo.Position != PlayerInfo.HouseLocation)
                {

                    return MovementActions.MoveTo(GameMap, PlayerInfo.HouseLocation - PlayerInfo.Position);
                }
                else
                {
                    return AIHelper.CreateUpgradeAction((UpgradeType)upgradeType);
                }


            }
        }

        /// <summary>
        /// Class that deals with the player collect actions
        /// </summary>
        public static class CollectActions
        {
            /// <summary>
            /// Move to rock and if close enough, collect
            /// </summary>
            public static string MoveToRock(Map map)
            { 
                Point position = new Point(2000, 2000);
                if (PlayerInfo.CarriedResources == PlayerInfo.CarryingCapacity)
                {
                    return MovementActions.MoveTo(map, PlayerInfo.HouseLocation - PlayerInfo.Position);
                }
                else
                {
                    foreach (Tile t in map.GetVisibleTiles())
                    {
                        if (t.TileType == TileContent.Resource)
                        {
                            if (Math.Pow(t.Position.X - PlayerInfo.Position.X, 2) + Math.Pow(t.Position.Y - PlayerInfo.Position.Y, 2)
                                < Math.Pow(position.X - PlayerInfo.Position.X, 2) + Math.Pow(position.Y - PlayerInfo.Position.Y, 2))
                            {
                                position = new Point(t.Position.X, t.Position.Y);
                            }
                        }
                    }
                    double distance = Math.Sqrt(Math.Pow((position.X - PlayerInfo.Position.X), 2) + (Math.Pow((position.Y - PlayerInfo.Position.Y), 2)));
                    if (distance > 1)
                    {
                        if(map.GetTileAt(PlayerInfo.Position.X + 1, PlayerInfo.Position.Y) == TileContent.Wall)
                        {
                            return AIHelper.CreateMeleeAttackAction(new Point(1, 0));
                        }
                        else if (map.GetTileAt(PlayerInfo.Position.X - 1, PlayerInfo.Position.Y) == TileContent.Wall)
                        {
                            return AIHelper.CreateMeleeAttackAction(new Point(-1, 0));
                        }
                        else if (map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y + 1) == TileContent.Wall)
                        {
                            return AIHelper.CreateMeleeAttackAction(new Point(0, 1));
                        }
                        else if(map.GetTileAt(PlayerInfo.Position.X, PlayerInfo.Position.Y - 1) == TileContent.Wall)
                        {
                            return AIHelper.CreateMeleeAttackAction(new Point(0, -1));
                        }
                        return MovementActions.MoveTo(map, position - PlayerInfo.Position);
                    }
                    else
                    {
                        return AIHelper.CreateCollectAction(position - PlayerInfo.Position);
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