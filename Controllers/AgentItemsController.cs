using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgentApi.Models;

namespace AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentItemsController : ControllerBase
    {
        private readonly AgentContext _context;

        public AgentItemsController(AgentContext context)
        {
            _context = context;
        }

        // GET: api/AgentItems
        [HttpGet]
        public IQueryable<AgentDto> GetAgentItems()
        {
            var agents = from a in _context.AgentItems
                         select new AgentDto()
                         {
                             id = a.id,
                             name = a.name,
                             os = a.os,
                             status = a.status,
                             type = a.type,
                             ip = a.ip,
                             location = a.location,
                             resources = a.resources.Select(r => r.Name).ToArray()
                         };
            return agents;
        }

        // GET: api/AgentItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentItem>> GetAgentItem(long id)
        {
            var agentItem = await _context.AgentItems.FindAsync(id);

            if (agentItem == null)
            {
                return NotFound();
            }

            return agentItem;
        }

        // PUT: api/AgentItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgentItem(long id, AgentDto agent)
        {
            if (id != agent.id)
            {
                return BadRequest();
            }

            AgentItem agentItem = new AgentItem()
            {
                id = agent.id,
                name = agent.name,
                os = agent.os,
                status = agent.status,
                type = agent.type,
                ip = agent.ip,
                location = agent.location,
                resources = new List<Resource>(),
            };

            _context.Entry(agentItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgentItemExists(id))
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

        // POST: api/AgentItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AgentItem>> PostAgentItem(AgentItem agentItem)
        {
            _context.AgentItems.Add(agentItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAgentItem), new { id = agentItem.id }, agentItem);
        }

        // DELETE: api/AgentItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgentItem(long id)
        {
            var agentItem = await _context.AgentItems.FindAsync(id);
            if (agentItem == null)
            {
                return NotFound();
            }

            _context.AgentItems.Remove(agentItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AgentItemExists(long id)
        {
            return _context.AgentItems.Any(e => e.id == id);
        }
    }
}
