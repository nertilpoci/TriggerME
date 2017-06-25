using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TriggerMe.Model;
using WebApplicationBasic.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using WebApplicationBasic;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using TriggerMe.DAL;
using TriggerMe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace TriggerMe.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Clients")]
    public class ClientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<IEnumerable<Client>> GetClient([FromQuery] bool includeTriggers=false)
        {
         
           
            return await _unitOfWork.Clients.GetAsync();

        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            var client = await _unitOfWork.Clients.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
    
        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient([FromRoute] int id, [FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           

            if (id != client.Id)
            {
                return BadRequest();
            }
            
            _unitOfWork.Clients.Update(client);

            try
            {
                await _unitOfWork.PersistAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Clients
        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            client.Identifier = Guid.NewGuid();
            _unitOfWork.Clients.Add(client);
          
            await _unitOfWork.PersistAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var client = await _unitOfWork.Clients.DeleteAsync(id);
            if (client == null )
            {
                return NotFound();
            }

            await _unitOfWork.PersistAsync();

            return Ok(client);
        }

        private bool ClientExists(int id)
        {
            return _unitOfWork.Clients.Count(z=>z.Id==id)>0;
        }
    }
}