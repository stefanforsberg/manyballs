using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public void MoveBall(string dir)
        {
            var ball = UserData[Context.ClientId];
            
            switch(dir)
            {
                case "r":
                    if (ball.LocX < 500) ball.LocX++;
                    break;
                case "l":
                    if(ball.LocX > 0) ball.LocX--;
                    break;
                case "u":
                    if(ball.LocY > 0) ball.LocY--;
                    break;
                case "d":
                    if(ball.LocY < 500) ball.LocY++;
                    break;
            }
            
            
            Clients.draw(UserData.ToArray());
        }

    }

    public class Deck
    {
        public string Name { get; set; }
    }
}