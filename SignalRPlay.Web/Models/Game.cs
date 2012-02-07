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
        public int LocX { get; set; }

        public int LocY { get; set; }

        public static Ball Random()
        {
            var random = new Random();
            return new Ball { LocX = random.Next(15, 450), LocY = random.Next(15, 450) };
        }
    }

    public class Game : Hub
    {
        static ConcurrentDictionary<string, Ball> UserData { get; set; }

        static Game()
        {
            UserData = new ConcurrentDictionary<string, Ball>();
        }

        //public void Join(string deckId)
        //{
        //    Caller.DeckId = deckId;

        //    //AddToGroup(deckId);
        //}

        public void Join()
        {
            UserData.AddOrUpdate(Context.ClientId, (k) => Ball.Random(), (k, v) => Ball.Random());
            Clients.draw(UserData.ToArray());
        }


        public void MoveBall(string dir)
        {
            var ball = UserData[Context.ClientId];
            
            switch(dir)
            {
                case "r":
                    ball.LocX++;
                    break;
                case "l":
                    ball.LocX--;
                    break;
                case "u":
                    ball.LocY--;
                    break;
                case "d":
                    ball.LocY++;
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