using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SignalR.Hubs;

namespace SignalRPlay.Web.Models
{
    public class Ball
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int LocX { get; set; }
        public int LocY { get; set; }
        public string LastDir { get; set; }
        public int Size { get; set; }

        public static Ball Random(string name, string color)
        {
            var random = new Random();

            return new Ball
                       {
                           Name = name,
                           Color = color,
                           LocX = random.Next(15, 450), 
                           LocY = random.Next(15, 450),
                           LastDir = "r",
                           Size = 40
                       };
        }
    }

    public static class ColliderHelper
    {
        public static bool SphereCollides(int x1, int y1, int r1, int x2, int y2, int r2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            var sumRadius = r1 + r2;
            return ((dx * dx) + (dy * dy) < sumRadius * sumRadius);
        }
    }

    public class World
    {
        static ConcurrentDictionary<string, Ball> _userData;

        public World()
        {
            _userData = new ConcurrentDictionary<string, Ball>();
        }

        public void AddBall(string clientId, string name, string color)
        {
            _userData.AddOrUpdate(clientId, (k) => Ball.Random(name, color), (k, v) => Ball.Random(name, color));
        }

        public IEnumerable<Ball> AllBalls()
        {
            return _userData.Select(b => b.Value);
        }

        public IEnumerable<KeyValuePair<string, Ball>> AllUserData()
        {
            return _userData.ToArray();
        }

        public Ball BallForUser(string clientId)
        {
            return _userData[clientId];
        }
    }

    public class Game : Hub, IDisconnect
    {
        static readonly World World;
        static bool _startedHeartbeat;

        static Game()
        {
            World = new World();
        }

        private void Heartbeat()
        {
            while(true)
            {
                Thread.Sleep(100);
                Clients.draw(World.AllUserData());                
            }
        }

        public void Join(string name, string color)
        {
            if (!_startedHeartbeat)
            {
                Task.Factory.StartNew(Heartbeat);
                _startedHeartbeat = true;
            }

            World.AddBall(Context.ClientId, name, color);
            Clients.showUsers(World.AllUserData());
        }

        public void HandleInput(string keyCode)
        {
            switch(keyCode)
            {
                case "32":
                    SomeOneSetUsUpTheBomb();
                    break;
                case "37":
                    MoveBall("l");
                    break;
                case "38":
                    MoveBall("u");
                    break;
                case "39":
                    MoveBall("r");
                    break;
                case "40":
                    MoveBall("d");
                    break;

            }
        }

        public void SomeOneSetUsUpTheBomb()
        {
            var ball = World.BallForUser(Context.ClientId);

            var x = ball.LocX - ball.Size / 2;
            var y = ball.LocY - ball.Size / 2;

            SendLogMessage(ball.Name + " set us up the bomb!");
            Clients.newBomb(x, y);

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                Clients.newBombExplode(x, y);

                BombExplodesOverBalls(x, y)
                    .ToList()
                    .ForEach(b =>
                        {
                            if(b.Size < 10)
                            {
                                SendLogMessage(b.Name + " was hit by bomb but is already so small :(");    
                            }
                            else
                            {
                                b.Size -= 5;
                                SendLogMessage(b.Name + " was hit by bomb!");
                            }
                            
                        });
            });
        }

        public void MoveBall(string dir)
        {
            var ball = World.BallForUser(Context.ClientId);

            var newPosX = ball.LocX;
            var newPosY = ball.LocY;

            switch(dir)
            {
                case "r":
                    if (newPosX < 500) newPosX++;
                    break;
                case "l":
                    if (newPosX > 0) newPosX--;
                    break;
                case "u":
                    if (newPosY > 0) newPosY--;
                    break;
                case "d":
                    if (newPosY < 500) newPosY++;
                    break;
            }

            if (Collides(newPosX, newPosY, ball.Size, true) == null)
            {
                ball.LocX = newPosX;
                ball.LocY = newPosY;
                ball.LastDir = dir;
            }
        }
        
        private void SendLogMessage(string message)
        {
            Clients.updateLog(message);
        }

        private Ball Collides(int newX, int newY, int currentRadius, bool ignoreSelf)
        {
            foreach (var ball in World.AllUserData())
            {
                if (ignoreSelf && ball.Key == Context.ClientId)
                {
                    continue;
                }

                if (SphereCollides(ball.Value.LocX, ball.Value.LocY, ball.Value.Size, newX, newY, currentRadius))
                {
                    return ball.Value;
                }
            }

            return null;
        }


        private IEnumerable<Ball> BombExplodesOverBalls(int x, int y)
        {
            foreach (var ball in World.AllUserData())
            {
                if (SphereCollides(ball.Value.LocX, ball.Value.LocY, ball.Value.Size, x, y, 22))
                {
                    yield return ball.Value;
                }
            }
        }

        private static bool SphereCollides(int x1, int y1, int r1, int x2, int y2, int r2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            var sumRadius = r1 + r2;
            return ((dx*dx) + (dy*dy) < sumRadius*sumRadius);
        }

        public void Disconnect()
        {
            
        }
    }
}