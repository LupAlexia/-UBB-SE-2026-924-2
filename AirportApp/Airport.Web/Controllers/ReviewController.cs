using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IRepository<int, Review> reviewRepository;
        private readonly IUserRepository userRepository;
        public ReviewController(IRepository<int, Review> reviewRepository, IUserRepository userRepository)
        {
            this.reviewRepository = reviewRepository;
            this.userRepository = userRepository;
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
        public async Task<ActionResult> CreateAsync([FromBody] CreateReviewDTO reviewCreationData)
        {
            var user = await userRepository.GetByIdAsync(reviewCreationData.userId);
            if (user == null)
            {
                return NotFound($"User with id {reviewCreationData.userId} not found.");
            }

            var review = new Review
            {
                User = user,
                Message = reviewCreationData.message,
                DutyFreeRating = reviewCreationData.dutyFreeRating,
                FlightExperienceRating = reviewCreationData.flightExperienceRating,
                StaffFriendlinessRating = reviewCreationData.staffFriendlinessRating,
                CleanlinessRating = reviewCreationData.cleanlinessRating
            };
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
