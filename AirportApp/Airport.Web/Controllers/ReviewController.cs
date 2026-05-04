using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IRepository<int, Review> reviewRepository;

        public ReviewController(IRepository<int, Review> reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllAsync()
        {
            IEnumerable<Review> reviews = await reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetByIdAsync(int id)
        {
            try
            {
                Review review = await reviewRepository.GetByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Review review)
        {
            int createdId = await reviewRepository.CreateNewEntityAsync(review);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Review review)
        {
            if (id != review.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await reviewRepository.UpdateByIdAsync(id, review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await reviewRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
