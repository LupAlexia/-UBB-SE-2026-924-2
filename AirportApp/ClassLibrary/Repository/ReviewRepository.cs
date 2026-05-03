using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain.Review;
using AirportApp.ClassLibrary.Entity.Domain;

using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class ReviewRepository : DatabaseRepository<int, Review>, IRepository<int, Review>
    {
        // private UserRepository _userRepository = new UserRepository();
        // public ReviewRepository() { }
        private readonly IRepository<int, User> userRepository;

        // Dependency Injection: Pass the repository in rather than creating it here
        public ReviewRepository(IRepository<int, User> userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public Review GetById(int reviewId)
        {
            string selectReviewQuery = "SELECT * FROM Review WHERE review_id = @id";
            SqlCommand selectCommand = new SqlCommand(selectReviewQuery);
            selectCommand.Parameters.AddWithValue("@id", reviewId);

            Review selectedReview = GetById(reviewId, selectCommand);

            if (selectedReview == null)
            {
                throw new KeyNotFoundException($"Review with id {reviewId} was not found.");
            }

            return selectedReview;
        }

        public IEnumerable<Review> GetAll()
        {
            string selectAllQuery = "SELECT * FROM Review";
            SqlCommand selectAllCommand = new SqlCommand(selectAllQuery);
            return GetAll(selectAllCommand);
        }

        public int CreateNewEntity(Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");
            }

            string newEntityQuery = "INSERT INTO Review " +
                "(user_id, message, duty_free_rating, flight_experience_rating, staff_friendliness_rating, cleanliness_rating) " +
                "OUTPUT INSERTED.Review_id " +
                "VALUES (@userId, @message, @dutyFree, @flightExp, @staff, @clean)";

            SqlCommand newEntityCommand = new SqlCommand(newEntityQuery);

            newEntityCommand.Parameters.AddWithValue("@userId", reviewElement.GetUser().UserId);
            newEntityCommand.Parameters.AddWithValue("@message", reviewElement.GetMessage());
            newEntityCommand.Parameters.AddWithValue("@dutyFree", reviewElement.GetDutyFreeRating());
            newEntityCommand.Parameters.AddWithValue("@flightExp", reviewElement.GetFlightExperienceRating());
            newEntityCommand.Parameters.AddWithValue("@staff", reviewElement.GetStaffFriendlinessRating());
            newEntityCommand.Parameters.AddWithValue("@clean", reviewElement.GetCleanlinessRating());

            int identificationNumber = Add(newEntityCommand, reviewElement);
            return identificationNumber;
        }

        public void UpdateById(int identificationNumber, Review reviewElement)
        {
            if (reviewElement == null)
            {
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");
            }

            string updateQuery = "UPDATE Review SET " +
                "user_id = @userId, " +
                "message = @message, " +
                "duty_free_rating = @dutyFree, " +
                "flight_experience_rating = @flightExp, " +
                "staff_friendliness_rating = @staff, " +
                "cleanliness_rating = @clean " +
                "WHERE review_id = @id";

            SqlCommand updateCommand = new SqlCommand(updateQuery);

            updateCommand.Parameters.AddWithValue("@id", identificationNumber);
            updateCommand.Parameters.AddWithValue("@userId", reviewElement.GetUser().UserId);
            updateCommand.Parameters.AddWithValue("@message", reviewElement.GetMessage());
            updateCommand.Parameters.AddWithValue("@dutyFree", reviewElement.GetDutyFreeRating());
            updateCommand.Parameters.AddWithValue("@flightExp", reviewElement.GetFlightExperienceRating());
            updateCommand.Parameters.AddWithValue("@staff", reviewElement.GetStaffFriendlinessRating());
            updateCommand.Parameters.AddWithValue("@clean", reviewElement.GetCleanlinessRating());

            UpdateById(identificationNumber, updateCommand, reviewElement);
        }

        public void DeleteById(int reviewId)
        {
            string deleteQuery = "DELETE FROM Review WHERE review_id = @id";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery);
            deleteCommand.Parameters.AddWithValue("@id", reviewId);

            DeleteById(reviewId, deleteCommand);
        }

        protected override Review MapRowToEntity(SqlDataReader reader)
        {
            int reviewId = reader.GetInt32(reader.GetOrdinal("review_id"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            string reviewMessage = reader.GetString(reader.GetOrdinal("message"));
            int dutyFreeRating = reader.GetInt32(reader.GetOrdinal("duty_free_rating"));
            int flightExperienceRating = reader.GetInt32(reader.GetOrdinal("flight_experience_rating"));
            int staffFriendlinessRating = reader.GetInt32(reader.GetOrdinal("staff_friendliness_rating"));
            int cleanlinessRating = reader.GetInt32(reader.GetOrdinal("cleanliness_rating"));

            User reviewUser = userRepository.GetById(userId);

            return new Review(reviewId, reviewUser, reviewMessage, dutyFreeRating, flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
        }

        protected override int GetEntityId(Review entity)
        {
            return entity.GetId();
        }
    }
}