using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SignalR;
using SignalR.Hubs;

namespace SignalRPlay.Web.Models
{
    public class UserIdClientIdFactory : IClientIdFactory
    {
        public string CreateClientId(HttpContextBase context)
        {
            return context.Request.Cookies["user"] == null ? Guid.NewGuid().ToString() : context.Request.Cookies["user"].Value;
        }
    }

    public class Ball
    {
        public const int MaxSize = 75;
        public string Name { get; set; }
        public string Color { get; set; }
        public int LocX { get; set; }
        public int LocY { get; set; }
        public string LastDir { get; set; }
        public int Size { get; set; }
        public int ActiveBombs { get; set; }
        public int Score { get; set; }
        public int Xspeed { get; set; }
        public int Yspeed { get; set; }

        public static Ball Random(string name, string color, int x, int y)
        {
            return new Ball
            {
                Name = name,
                ActiveBombs = 0,
                Color = color,
                LocX = x, 
                LocY = y,
                LastDir = "r",
                Size = 20,
                Score = 0,
                Xspeed = 0,
                Yspeed = 0
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

    public struct Pos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }
    }

    public class World
    {
        static ConcurrentDictionary<string, Ball> _userData;
        static ConcurrentDictionary<Guid, Pos> _food;
        public static bool IsRunning;
        public static int Width = 800;
        public static int Height = 500;

        public World()
        {
            _userData = new ConcurrentDictionary<string, Ball>();
            _food = new ConcurrentDictionary<Guid, Pos>();
            IsRunning = false;
        }

        public void AddBall(string clientId, string color, int x, int y)
        {
            _userData.AddOrUpdate(clientId, (k) => Ball.Random(clientId, color, x, y), (k, v) => Ball.Random(clientId, color, x, y));
        }

        public void AddFood(Pos pos)
        {
            _food.AddOrUpdate(Guid.NewGuid(), (k) => pos, (k, v) => pos);
        }

        public IEnumerable<KeyValuePair<Guid, Pos>> AllFood()
        {
            return _food.ToArray();
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

        public void RemoveFood(Guid id)
        {
            Pos removedFood;
            _food.TryRemove(id, out removedFood);
        }

        public void ResetAll()
        {
            IsRunning = false;
            _userData.Clear();
            _food.Clear();
        }
    }

    public class Game : Hub, IDisconnect
    {
        public static readonly World World;

        static Game()
        {
            World = new World();
        }

        private void Heartbeat()
        {
            int foodCounter = 0;

            while (World.IsRunning)
            {
                foodCounter++;
                Thread.Sleep(50);
                Clients.draw(World.AllUserData());

                if (foodCounter <= 200) continue;

                var pos = GetNonCollidingPosition(15);
                World.AddFood(pos);
                Clients.addFood(pos.X, pos.Y);
                foodCounter = 0;
            }
        }

        private void Gameloop()
        {
            while (World.IsRunning)
            {
                foreach (var ball in World.AllBalls())
                {
                    var newPosX = ball.LocX + ball.Xspeed;
                    var newPosY = ball.LocY + ball.Yspeed;

                    //if (ball.Name.ToLower() == "hanna")
                    //{
                    //    SendLogMessage(string.Format("{0} - xspeed: {1} x: {2}, newx: {3}", ball.Name, ball.Xspeed, ball.LocX, newPosX));
                    //}
                    

                    if (newPosX >= World.Width) newPosX = World.Width;
                    if (newPosX <= 0) newPosX = 0;
                    if (newPosY <= 0) newPosY = 0;
                    if (newPosY >= World.Height) newPosY = World.Height;

                    var c = Collides(newPosX, newPosY, ball.Size, ball.Name);

                    if(c != null)
                    {
                        SendLogMessage(ball.Name + " coll with " + c.Name);

                    }

                    if (Collides(newPosX, newPosY, ball.Size, ball.Name) == null)
                    {
                        ball.LocX = newPosX;
                        ball.LocY = newPosY;

                        //if (ball.Name.ToLower() == "hanna")
                        //{
                        //    SendLogMessage(string.Format("{0} - xspeed: {1} x: {2}, newx: {3}", ball.Name, ball.Xspeed, ball.LocX, newPosX))
                        //}
                    
                    }

                    

                    EatsFood(ball)
                        .ToList()
                        .ForEach(f =>
                        {
                            World.RemoveFood(f.Key);
                            Clients.foodEaten(f.Value.X, f.Value.Y);
                            SendLogMessage(ball.Name + " ate some food!");

                            if (ball.Size < Ball.MaxSize)
                            {
                                ball.Size += 5;
                            }

                            ball.Score++;

                            Clients.showUsers(World.AllUserData());
                        });

                    Thread.Sleep(50);
                }
            }

        }

        public void Join(string color)
        {
            if (!World.IsRunning)
            {
                World.IsRunning = true;
                Task.Factory.StartNew(Gameloop);
                Task.Factory.StartNew(Heartbeat);
                
            }

            var pos = GetNonCollidingPosition(50);

            World.AddBall(Context.ClientId, color, pos.X, pos.Y);
            World.AllFood().ToList().ForEach(f => Caller.addFood(f.Value.X, f.Value.Y));
            Caller.joined(World.AllUserData());
            Clients.showUsers(World.AllUserData());

        }

        private Pos GetNonCollidingPosition(int size)
        {
            var random = new Random();

            var x = random.Next(15, World.Width-50);
            var y = random.Next(15, World.Height - 50);

            while (Collides(x, y, size, Context.ClientId) != null)
            {
                x = random.Next(15, World.Width - 50);
                y = random.Next(15, World.Height - 50);
            }

            return new Pos { X = x, Y = y, Size = size};
        }

        public void HandleInput(string keyCode, int factor)
        {
            switch(keyCode)
            {
                case "32":
                    SomeOneSetUsUpTheBomb();
                    break;
                case "65":
                    MoveBall("l", factor);
                    break;
                case "87":
                    MoveBall("u", factor);
                    break;
                case "68":
                    MoveBall("r", factor);
                    break;
                case "83":
                    MoveBall("d", factor);
                    break;

            }
        }

        public void SomeOneSetUsUpTheBomb()
        {
            var clientId = Context.ClientId;
            var ball = World.BallForUser(clientId);

            if(ball.ActiveBombs >= MaxActiveBombs)
            {
                return;
            }

            ball.ActiveBombs++;

            var x = ball.LocX - ball.Size / 2;
            var y = ball.LocY - ball.Size / 2;
            var xspeed = 0;
            var yspeed = 0;

            var bombId = Guid.NewGuid();
            switch(ball.LastDir)
            {
                case "r":
                    xspeed = -2;
                    break;
                case "l":
                    xspeed = 2;
                    break;
                case "u":
                    yspeed = 2;
                    break;
                case "d":
                    yspeed = -2;
                    break;
            }
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                Clients.newBombExplode(x, y, bombId);

                World.BallForUser(clientId).ActiveBombs--;

                BombExplodesOverBalls(x +22 + 90 * xspeed, y + 22 + 90 * yspeed)
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

        public const int MaxActiveBombs = 3;

        public void MoveBall(string dir, int factor)
        {
            const int BaseSpeed = 8;

            var ball = World.BallForUser(Context.ClientId);

            if(factor > 0 )
            {
                ball.LastDir = dir;    
            }
            


            switch(dir)
            {
                case "r":
                    ball.Xspeed = BaseSpeed * factor;
                    break;
                case "l":
                    ball.Xspeed = -BaseSpeed * factor;
                    break;
                case "u":
                    ball.Yspeed = -BaseSpeed * factor;
                    break;
                case "d":
                    ball.Yspeed = BaseSpeed * factor;
                    break;
            }
        }
        
        private void SendLogMessage(string message)
        {
            Clients.updateLog(message);
        }

        private Ball Collides(int newX, int newY, int currentRadius, string ballIdToIgnore)
        {
            foreach (var ball in World.AllUserData())
            {
                if (!string.IsNullOrEmpty(ballIdToIgnore) && ball.Key == ballIdToIgnore)
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

        private IEnumerable<KeyValuePair<Guid, Pos>> EatsFood(Ball ballToCheck)
        {
            foreach (var food in World.AllFood())
            {
                if (SphereCollides(food.Value.X, food.Value.Y, food.Value.Size, ballToCheck.LocX, ballToCheck.LocY, ballToCheck.Size))
                {
                    yield return food;
                }
            }
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