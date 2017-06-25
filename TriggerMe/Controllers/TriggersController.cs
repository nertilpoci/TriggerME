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
using TriggerMe.DAL;

namespace TriggerMe.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Triggers")]
   
    public class TriggersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TriggersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

     

        // GET: api/Triggers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTriggerMessages([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client =  await _unitOfWork.Clients.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(await _unitOfWork.Triggers.GetAsync(m => m.ClientId == client.Id));
        }

        // PUT: api/Triggers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTriggerMessage([FromRoute] int id, [FromBody] TriggerMessage triggerMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != triggerMessage.Id)
            {
                return BadRequest();
            }

            _unitOfWork.Triggers.Update(triggerMessage);

            try
            {
                await _unitOfWork.PersistAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TriggerMessageExists(id))
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

        // POST: api/Triggers
        [HttpPost]
        public async Task<IActionResult> PostTriggerMessage([FromBody] TriggerMessage triggerMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            triggerMessage.Secret = Guid.NewGuid().ToString();
            _unitOfWork.Triggers.Add(triggerMessage);
            await _unitOfWork.PersistAsync();

            return CreatedAtAction("GetTriggerMessage", new { id = triggerMessage.Id }, triggerMessage);
        }

        // DELETE: api/Triggers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTriggerMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var triggerMessage = await _unitOfWork.Triggers.DeleteAsync(id);
            if (triggerMessage == null)
            {
                return NotFound();
            }


            await _unitOfWork.PersistAsync();

            return Ok(triggerMessage);
        }

        private bool TriggerMessageExists(int id)
        {
            return _unitOfWork.Triggers.Count(z => z.Id == id) > 0;
        }
    }
}