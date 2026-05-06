using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FAQController : ControllerBase
    {
        private readonly IFAQRepository faqRepository;

        public FAQController(IFAQRepository faqRepository)
        {
            this.faqRepository = faqRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQEntry>>> GetAllAsync()
        {
            IEnumerable<FAQEntry> entries = await faqRepository.GetAllAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQEntry>> GetByIdAsync(int id)
        {
            try
            {
                FAQEntry entry = await faqRepository.GetByIdAsync(id);
                return Ok(entry);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("by-category")]
        public async Task<ActionResult<IEnumerable<FAQEntry>>> GetByCategoryAsync([FromQuery] FAQCategoryEnum category)
        {
            List<FAQEntry> entries = await faqRepository.GetByCategoryAsync(category);
            return Ok(entries);
        }

        [HttpPost("{id}/increment-view")]
        public async Task<ActionResult> IncrementViewCountAsync(int id)
        {
            await faqRepository.IncrementViewCountAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/increment-helpful")]
        public async Task<ActionResult> IncrementHelpfulAsync(int id)
        {
            await faqRepository.IncrementWasHelpfulVotesAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/increment-not-helpful")]
        public async Task<ActionResult> IncrementNotHelpfulAsync(int id)
        {
            await faqRepository.IncrementWasNotHelpfulVotesAsync(id);
            return NoContent();
        }

        //adaugate de dede pentru proxy 
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] FAQEntry entry)
        {
            int createdId = await faqRepository.CreateNewEntityAsync(entry);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, entry);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] FAQEntry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }
            await faqRepository.UpdateByIdAsync(id, entry);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await faqRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
