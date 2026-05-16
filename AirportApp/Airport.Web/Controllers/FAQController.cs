using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FAQController : ControllerBase
    {
        private readonly IFAQService faqService;

        public FAQController(IFAQService faqService)
        {
            this.faqService = faqService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQEntry>>> GetAllAsync()
        {
            List<FAQEntry> entries = await faqService.GetAllAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQEntry>> GetByIdAsync(int id)
        {
            try
            {
                List<FAQEntry> entries = await faqService.GetAllAsync();
                FAQEntry entry = entries.Find(e => e.Id == id)
                    ?? throw new KeyNotFoundException();
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
            List<FAQEntry> entries = await faqService.GetByCategoryAsync(category);
            return Ok(entries);
        }

        [HttpPost("{id}/increment-view")]
        public async Task<ActionResult> IncrementViewCountAsync(int id)
        {
            try
            {
                List<FAQEntry> entries = await faqService.GetAllAsync();
                FAQEntry entry = entries.Find(e => e.Id == id)
                    ?? throw new KeyNotFoundException();
                await faqService.IncrementViewCountAsync(entry);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/increment-helpful")]
        public async Task<ActionResult> IncrementHelpfulAsync(int id)
        {
            try
            {
                List<FAQEntry> entries = await faqService.GetAllAsync();
                FAQEntry entry = entries.Find(e => e.Id == id)
                    ?? throw new KeyNotFoundException();
                await faqService.IncrementWasHelpfulVotesAsync(entry);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/increment-not-helpful")]
        public async Task<ActionResult> IncrementNotHelpfulAsync(int id)
        {
            try
            {
                List<FAQEntry> entries = await faqService.GetAllAsync();
                FAQEntry entry = entries.Find(e => e.Id == id)
                    ?? throw new KeyNotFoundException();
                await faqService.IncrementWasNotHelpfulVotesAsync(entry);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] FAQEntry entry)
        {
            await faqService.AddFAQEntryAsync(entry);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = entry.Id }, entry);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] FAQEntry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await faqService.EditFAQEntryAsync(entry, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await faqService.DeleteFAQEntryAsync(id);
            return NoContent();
        }
    }
}