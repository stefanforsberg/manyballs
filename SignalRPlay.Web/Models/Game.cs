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

        public static Ball Random(string name, string color)
        {
            var random = new Random();

            return new Ball
                       {
                           Name = name,
                           Color = color,
                           LocX = random.Next(15, 450), 
                           LocY = random.Next(15, 450)
                       };
        }
    }

    public class Game : Hub
    {
        static ConcurrentDictionary<string, Ball> UserData { get; set; }

        static Game()
        {
            UserData = new ConcurrentDictionary<string, Ball>();
        }

        public void Join(string name, string color)
        {
            UserData.AddOrUpdate(Context.ClientId, (k) => Ball.Random(name, color), (k, v) => Ball.Random(name, color));
            Clients.showUsers(UserData.ToArray());
            Clients.draw(UserData.ToArray());
        }

        public void SomeOneSetUsUpTheBomb()
        {
            var ball = UserData[Context.ClientId];

            var x = ball.LocX - 22;
            var y = ball.LocY - 22;

            Clients.newBomb(x, y);
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Clients.newBombExplode(x, y);    
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

            if (!Collides(newPosX, newPosY))
            {
                ball.LocX = newPosX;
                ball.LocY = newPosY;
            }
            
            Clients.draw(UserData.ToArray());
        }

        private bool Collides(int newX, int newY)
        {
            const int radii = 40 + 40;

            foreach(var ball in UserData.Where(c => c.Key != Context.ClientId))
            {
                var dx = ball.Value.LocX - newX;
                var dy = ball.Value.LocY - newY;
                
                if ( ( dx * dx )  + ( dy * dy ) < radii * radii )
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Deck
    {
        public string Name { get; set; }
    }
}