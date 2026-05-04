using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionTreeController : ControllerBase
    {
        private readonly IRepository<int, FAQNode> decisionTreeRepository;

        public DecisionTreeController(IRepository<int, FAQNode> decisionTreeRepository)
        {
            this.decisionTreeRepository = decisionTreeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQNode>>> GetAllAsync()
        {
            IEnumerable<FAQNode> nodes = await decisionTreeRepository.GetAllAsync();
            return Ok(nodes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQNode>> GetByIdAsync(int id)
        {
            try
            {
                FAQNode node = await decisionTreeRepository.GetByIdAsync(id);
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
            int createdId = await decisionTreeRepository.CreateNewEntityAsync(node);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, node);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] FAQNode node)
        {

            await decisionTreeRepository.UpdateByIdAsync(id, node);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await decisionTreeRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
