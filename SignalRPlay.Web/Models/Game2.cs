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

    public class User
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public bool WantsToPlay { get; set; }
        public Guid GameRoomId { get; set; }
    }

    public class Game2 : Hub
    {
        static readonly ConcurrentDictionary<string, User> _users;

        static Game2()
        {
            _users = new ConcurrentDictionary<string, User>();
        }

        public void Join()
        {
            _users.TryAdd(Context.ClientId, new User { Name = Context.ClientId, WantsToPlay = true });

            Caller.UserName = Context.ClientId;
            Clients.updateUsers(_users.ToArray());
        }

        public void Challenge(string challengedUser)
        {
            Clients[challengedUser].challengeRecieved(
                new { Name = Caller.UserName, ChallengerId = Caller.UserName }
                );
        }


        public void PostMessage(string message)
        {
            Clients.addMessage(new {Message = message});
        }

        public void StartGame(string challengerId)
        {
            PostMessage(string.Format("{0} startade spel med {1}", Context.ClientId, challengerId));

            _users[challengerId].WantsToPlay = false;
            _users[Context.ClientId].WantsToPlay = false;

            Clients.updateUsers(_users.ToArray());

            Clients[challengerId].gameStarted(Context.ClientId);
            Caller.gameStarted(challengerId);
        }

        public void ChallengerReady(string challengerId, int tile)
        {
            Clients[challengerId].tileClicked(new { Tile = tile });
        }

        //public void ClickTile(int id)
        //{
        //    Clients.tileClicked(new { Tile = id });
        //}
    }
}