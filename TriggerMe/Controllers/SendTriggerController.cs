using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TriggerMe.DAL;
using TriggerMe.Model;
using WebApplicationBasic.Data;
using WebApplicationBasic.Hubs;

namespace TriggerMe.Controllers
{
    [Produces("application/json")]
    [Route("api/SendTrigger")]
    public class SendTriggerController:Controller
    {
        private IHubContext _clientHub;
        private IUnitOfWork _unitOfwork;
        public SendTriggerController(IUnitOfWork unitOfwork, IConnectionManager connectionManager)
        {
            _unitOfwork = unitOfwork;
            _unitOfwork.SetTenancy(false);
          
            _clientHub = connectionManager.GetHubContext<ClientHub>();

        }
       
        [HttpPost("{identifier}/{secret}")]
        public async Task<IActionResult> Post([FromRoute] string identifier, [FromRoute] string secret,[FromBody] object obj)
        {
            var status = await SendTrigger(identifier, secret, obj);
            if (!status) return NotFound();
            return Ok();
                
        }
        [HttpGet("{identifier}/{secret}")]
        public async Task<IActionResult> Get([FromRoute] string identifier, [FromRoute] string secret)
        {
            var status = await SendTrigger(identifier, secret);
            if (!status) return NotFound();
            return Ok();
        }
        async Task<bool> SendTrigger(string identifier,string secret,object obj=null)
        {
            var guid = Guid.Parse(identifier);
          
            var connections = await _unitOfwork.Connections.GetAsync(c => c.Client.Identifier.Equals(guid));
            var trigger = await _unitOfwork.Triggers.SingleAsync(z => z.Secret == secret && z.Client.Identifier.Equals(guid));
            if (trigger == null) return false;
            _clientHub.Clients.Clients(connections.Select(z => z.ConnectionID).ToList()).sendTrigger(new TriggerMessage { Id = trigger.Id, Secret = trigger.Secret, Description = trigger.Description, Name = trigger.Name, TenantId = trigger.TenantId, ClientId = trigger.ClientId });
            return true;
        }
    }
}
