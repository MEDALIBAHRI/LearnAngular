using System;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            this._presenceTracker = presenceTracker;
            
        }
        public override async Task OnConnectedAsync(){
            string username =Context.User.GetUsername();
            
           var isOnline =  await this._presenceTracker.UserConnected(username, Context.ConnectionId);
            if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", username);
            await Clients.Caller.SendAsync("GetOnlineUsers", await _presenceTracker.GetOnlineUsers());

        }
        public override async Task OnDisconnectedAsync(Exception exception){
             string username =Context.User.GetUsername();
            
            var isOffline =   await this._presenceTracker.UserDisconnected(username, Context.ConnectionId);
            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline", username);

            await base.OnDisconnectedAsync(exception);
        }
    }
}