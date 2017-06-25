using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TriggerMe.Model;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationBasic.Hubs
{
    [Authorize]
    public class WebUserHub : Hub
    {
        public static Dictionary<string,HashSet<string>> ConnectedUsers=new Dictionary<string, HashSet<string>>();
        private readonly UserManager<ApplicationUser> _userManager;
        private  string userid;
        public WebUserHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            
        }

        public void SendMessage(string message)
        {

            var user = _userManager.GetUserAsync(Context.User as ClaimsPrincipal).Result;
            var connections = ConnectedUsers[user.Id].ToList();
            
            Clients.Clients(connections).messageReceived(message);
        }
        public void UpdateClient(string userId, Client client)
        {

            if(ConnectedUsers.ContainsKey(userId))
            {
                var connections = ConnectedUsers[userid].ToList();

                Clients.Clients(connections).clientUpdated(client);
            }
           
        }
        public void UpdateClient( Client client)
        {

            var user = _userManager.GetUserAsync(Context.User as ClaimsPrincipal).Result;
          
                var connections = ConnectedUsers[user.Id].ToList();

            Clients.Clients(connections).clientUpdated(client);
        }
        public override   Task OnConnected()
        {
            Debug.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;
            var user = _userManager.GetUserId(Context.User as ClaimsPrincipal);
            if(ConnectedUsers.ContainsKey(user))
            {
                ConnectedUsers[user].Add(connectionId);
            }
            else
            {
                ConnectedUsers.Add(user, new HashSet<string> { connectionId });
            }
           
            return (base.OnConnected());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Hub OnDisconnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;
            var user = _userManager.GetUserId(Context.User as ClaimsPrincipal);
            if (ConnectedUsers.ContainsKey(user))
            {
                ConnectedUsers[user].Remove(connectionId);
            }
           

            return base.OnDisconnected(stopCalled);
        }


        public override Task OnReconnected()
        {
            Debug.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;
            var user = _userManager.GetUserId(Context.User as ClaimsPrincipal);
            if (ConnectedUsers.ContainsKey(user))
            {
                ConnectedUsers[user].Add(connectionId);
            }
            else
            {
                ConnectedUsers.Add(user, new HashSet<string> { connectionId });
            }

            return base.OnReconnected();
        }
    }
}
