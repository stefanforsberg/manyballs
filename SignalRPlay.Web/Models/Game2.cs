using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace SignalRPlay.Web.Models
{
    public class User
    {
        public string Name { get; set; }
        public bool WantsToPlay { get; set; }
    }

    public class Game2 : Hub
    {
        static readonly ConcurrentDictionary<string, User> _users;

        static Game2()
        {
            _users = new ConcurrentDictionary<string, User>();
        }

        public void Join(string userName)
        {
            _users.TryAdd(userName, new User {Name = userName, WantsToPlay = false});

            Caller.UserName = userName;
            Clients.activateChat();
            Clients.updateUsers(_users.ToArray());
        }

        public void PostMessage(string message)
        {
            Clients.addMessage(new {User = Caller.UserName, Message = message});
        }

        public void ClickTile(int id)
        {
            Clients.tileClicked(new { Tile = id });
        }
    }
}