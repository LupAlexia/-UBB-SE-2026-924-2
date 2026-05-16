using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionTreeController : ControllerBase
    {
        private readonly IDecisionTreeService decisionTreeService;

        public DecisionTreeController(IDecisionTreeService decisionTreeService)
        {
            this.decisionTreeService = decisionTreeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQNode>>> GetAllAsync()
        {
            IEnumerable<FAQNode> nodes = await decisionTreeService.GetAllNodesAsync();
            return Ok(nodes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQNode>> GetByIdAsync(int id)
        {
            try
            {
                FAQNode node = await decisionTreeService.GetNodeByIdAsync(id);
                return Ok(node);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] FAQNode node)
        {
            int createdId = await decisionTreeService.CreateNodeAsync(node);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, node);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] FAQNode node)
        {
            await decisionTreeService.UpdateNodeAsync(id, node);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await decisionTreeService.DeleteNodeAsync(id);
            return NoContent();
        }
    }
}