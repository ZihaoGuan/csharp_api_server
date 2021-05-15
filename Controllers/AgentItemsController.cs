using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgentApi.Models;
using Newtonsoft.Json;
using System.IO;

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
        public async Task<ActionResult<AgentDto>> PutAgentItem(long id)
        {
            if (!AgentItemExists(id))
            {
                return BadRequest();
            }

            //read raw and deserialize it to AgentDto object
            if (Request.Body.CanSeek)
            {
                // Reset the position to zero to read from the beginning.
                Request.Body.Position = 0;
            }
            var rawRequestBody = new StreamReader(Request.Body).ReadToEnd();

            AgentDto agent = JsonConvert.DeserializeObject<AgentDto>(rawRequestBody);


            _context.AgentItems.Load();

            if (id != agent.id)
            {
                return BadRequest();
            }

            AgentItem agentItem = null;

            List<Resource> resources = null;

            //find agentItem in database with given id, then load its resources
            foreach (var a in _context.AgentItems.Local)
            {
                if (a.id == id)
                {
                    agentItem = _context.AgentItems.Single(a => a.id == id);
                    _context.Entry(agentItem).Collection(a => a.resources).Load();
                    resources = a.resources;
                    foreach (var s in resources)
                        _context.Resources.Remove(s);
                    break;
                }
            }

            await _context.SaveChangesAsync();

            foreach (var s in agent.resources)
            {
                var resource = new Resource() { Name = s };
                _context.Resources.Add(resource);
                resources.Add(resource);
            }

            await _context.SaveChangesAsync();

            _context.Entry(agentItem).State = EntityState.Detached;
            agentItem = new AgentItem()
            {
                id = agent.id,
                name = agent.name,
                os = agent.os,
                status = agent.status,
                type = agent.type,
                ip = agent.ip,
                location = agent.location,
                resources = resources,
            };

            _context.Entry(agentItem).State = EntityState.Modified;
            _context.AgentItems.Update(agentItem);
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

            return new AgentDto()
            {
                id = agentItem.id,
                name = agentItem.name,
                os = agentItem.os,
                status = agentItem.status,
                type = agentItem.type,
                ip = agentItem.ip,
                location = agentItem.location,
                resources = agentItem.resources.Select(r => r.Name).ToArray()
            };
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

        // // DELETE: api/AgentItems/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteAgentItem(long id)
        // {
        //     var agentItem = await _context.AgentItems.FindAsync(id);
        //     if (agentItem == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.AgentItems.Remove(agentItem);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool AgentItemExists(long id)
        {
            return _context.AgentItems.Any(e => e.id == id);
        }
    }
}
