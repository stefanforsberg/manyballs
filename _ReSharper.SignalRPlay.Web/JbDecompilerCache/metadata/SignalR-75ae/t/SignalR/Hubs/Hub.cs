// Type: SignalR.Hubs.Hub
// Assembly: SignalR, Version=0.3.5.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Development\SignalRPlay\SignalRPlay.Web\packages\SignalR.Server.0.3.5\lib\net40\SignalR.dll

using SignalR;
using System.Threading.Tasks;

namespace SignalR.Hubs
{
    public abstract class Hub : IHub
    {
        public dynamic Clients { get; }

        #region IHub Members

        public IClientAgent Agent { get; set; }
        public dynamic Caller { get; set; }
        public HubContext Context { get; set; }
        public IGroupManager GroupManager { get; set; }

        #endregion

        public Task AddToGroup(string groupName);
        public Task RemoveFromGroup(string groupName);
        public static Task Invoke<T>(string method, params object[] args) where T : IHub;
        public static Task Invoke(string hubName, string method, params object[] args);
        public static dynamic GetClients<T>() where T : IHub;
        public static dynamic GetClients(string hubName);
    }
}
