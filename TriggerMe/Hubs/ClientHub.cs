using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TriggerMe;
using TriggerMe.Model;
using WebApplicationBasic.Data;

namespace WebApplicationBasic.Hubs
{
    [Authorize]
    public class ClientHub : Hub
    {

        private readonly  ApplicationDbContext _context;
        private IHubContext _hub;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConnectionManager connectionManager)
        {
            _context = context;
            _userManager = userManager;
            _hub = connectionManager.GetHubContext<WebUserHub>();
           
        }
       

       
        public override Task OnConnected()
        {
            Debug.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;           
            var clientIdentifier = Context.Headers["ClientIdentifier"].ToString();
                var client = _context.Client.FirstOrDefault(u => u.Identifier.ToString() == clientIdentifier);
            var webusers = WebUserHub.ConnectedUsers;
            if (client != null)
            {
                var user = _userManager.GetUserId(Context.User as ClaimsPrincipal);
                client.Connections.Add(new TriggerMe.Model.Connection { Connected = true, ConnectionID = connectionId, UserAgent = "" });
                client.IsOnline = true;
                _context.SaveChanges();
                if(WebUserHub.ConnectedUsers.ContainsKey(user))
                {
                    
                    _hub.Clients.Clients(WebUserHub.ConnectedUsers[user].ToList()).clientUpdated(new Client { Id=client.Id, Identifier=client.Identifier, Description=client.Description,TenantId=client.TenantId, Name=client.Name, IsOnline=client.IsOnline });
                }
                
            }
 
            return (base.OnConnected());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Hub OnDisconnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;
            var clientIdentifier = Context.Headers["ClientIdentifier"].ToString();
            var connection = _context.Connections.FirstOrDefault(u => u.ConnectionID.ToString() == connectionId);

            if (connection != null)
            {
                var webusers = WebUserHub.ConnectedUsers;
                var client = connection.Client;
                var user = _userManager.GetUserId(Context.User as ClaimsPrincipal);
                client.IsOnline = client.Connections.Count > 1;
                _context.Connections.Remove(connection);
                _context.SaveChanges();
                if (WebUserHub.ConnectedUsers.ContainsKey(user))
                {

                    _hub.Clients.Clients(WebUserHub.ConnectedUsers[user].ToList()).clientUpdated(new Client { Id = client.Id, Identifier = client.Identifier, Description = client.Description, Name = client.Name, IsOnline = client.IsOnline });
                }
            }
          
          
            return base.OnDisconnected(stopCalled);
        }


        public override Task OnReconnected()
        {
            Debug.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
            string connectionId = Context.ConnectionId;
            //var connection = _context.Connections.FirstOrDefault(u => u.ConnectionID.ToString() == connectionId);

            //if (connection != null)
            //{
            //    connection.Connected = true;
            //    _context.SaveChanges();
            //}
            return base.OnReconnected();
        }
    }
}
