using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

    public class Game : Hub, IDisconnect
    {
        static ConcurrentDictionary<string, Ball> UserData { get; set; }
        static bool _startedHeartbeat;

        static Game()
        {
            UserData = new ConcurrentDictionary<string, Ball>();
        }

        private void Heartbeat()
        {
            while(true)
            {
                Thread.Sleep(100);
                Clients.draw(UserData.ToArray());                
            }
        }

        public void Join(string name, string color)
        {
            if (!_startedHeartbeat)
            {
                Task.Factory.StartNew(Heartbeat);
                _startedHeartbeat = true;
            }

            UserData.AddOrUpdate(Context.ClientId, (k) => Ball.Random(name, color), (k, v) => Ball.Random(name, color));
            Clients.showUsers(UserData.ToArray());
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
            var ball = UserData[Context.ClientId];

            var x = ball.LocX - ball.Size / 2;
            var y = ball.LocY - ball.Size / 2;

            Clients.newBomb(x, y);

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                Clients.newBombExplode(x, y);
                var ballColliding = Collides(x, y, 44, false);

                if(ballColliding != null)
                {
                    ballColliding.Size -= 5;    
                }
            });
        }

        public void MoveBall(string dir)
        {
            var ball = UserData[Context.ClientId];

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

        private Ball Collides(int newX, int newY, int currentSize, bool ignoreSelf)
        {
            foreach(var ball in UserData)
            {
                if(ignoreSelf && ball.Key == Context.ClientId)
                {
                    continue;
                }

                var dx = ball.Value.LocX - newX;
                var dy = ball.Value.LocY - newY;
                var sumRadius = ball.Value.Size + currentSize;
                if ((dx * dx) + (dy * dy) < sumRadius * sumRadius)
                {
                    return ball.Value;
                }
            }

            return null;
        }

        public void Disconnect()
        {
            
        }
    }

    public class Deck
    {
        public string Name { get; set; }
    }
}