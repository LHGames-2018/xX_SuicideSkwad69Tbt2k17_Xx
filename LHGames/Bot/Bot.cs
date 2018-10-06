using System;
using System.Collections.Generic;
using LHGames.Helper;

namespace LHGames.Bot
{
    internal class Bot
    {
        enum ETATS { COLLECTER, ATTAQUER, DEFENDRE, UPGRADE, VOLER, RECHERCHER };
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
            MovementActions movement = new MovementActions();
            PlayerActions actions = new PlayerActions(map);
            CollectActions collection = new CollectActions();

            // Scanning the map
            foreach(Tile t in map.GetVisibleTiles())
            {
                if(t.TileType == TileContent.Player) // 1e priorite : attaquer et defendre
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
                else if (t.TileType == TileContent.Resource) // 2e priorite : collecter des ressources
                {
                    presentState = (int)ETATS.COLLECTER;
                }
                else // Si on ne voit rien de pertinent // derniere priorite : rechercher des ressources
                {
                    presentState = (int)ETATS.RECHERCHER;
                }
            }

            switch (presentState)
            {
                case (int)ETATS.COLLECTER:
                    //collection.Collecter();
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
                case (int)ETATS.RECHERCHER:
                    //actions.Rechercher(); // plus rien sur la map visible
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
            private MovementActions movement;

            public MovementActions Movement
            {
                get => movement;
                set
                {
                    movement = value;
                }
            }

            private Map gameMap;

            public Map GameMap
            {
                get { return gameMap; }
                set { gameMap = value; }
            }


            public PlayerActions(Map map)
            {
                Movement = new MovementActions();
                GameMap = map;
            }

            //Find the distance to nearest enemy
            public void FindEnemyAndMove(IEnumerable<IPlayer> visiblePlayers)
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

            public void Attack(Point direciton)
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