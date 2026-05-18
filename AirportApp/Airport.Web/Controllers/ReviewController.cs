using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly IUserService userService;

        public ReviewController(IReviewService reviewService, IUserService userService)
        {
            this.reviewService = reviewService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllAsync()
        {
            IEnumerable<Review> reviews = await reviewService.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetByIdAsync(int id)
        {
            try
            {
                Review review = await reviewService.GetByIdAsync(id);
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
            User user = await userService.GetByIdAsync(reviewCreationData.userId);
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

            int createdId = await reviewService.AddAsync(review);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Review review)
        {
            if (id != review.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await reviewService.UpdateByIdAsync(id, review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await reviewService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("calculate-average")]
        public async Task<ActionResult<float>> CalculateAverageAsync([FromBody] Review review)
        {
            float average = await reviewService.CalculateAverageRatingAsync(review);
            return Ok(average);
        }
    }
}