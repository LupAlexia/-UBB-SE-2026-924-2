using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using Microsoft.Data.SqlClient;

using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class FAQRepository : DatabaseRepository<int, FAQEntry>, IFAQRepository
    {
    public FAQRepository()
        {
        }

    public FAQEntry GetById(int askedQuestionId)
        {
            SqlCommand selectByIdCommand = new SqlCommand("SELECT * FROM FAQEntry WHERE FAQentry_id = @id");
            selectByIdCommand.Parameters.AddWithValue("@id", askedQuestionId);

            FAQEntry frequentlyAskedQuestion = GetById(askedQuestionId, selectByIdCommand);

            if (frequentlyAskedQuestion == null)
            {
                throw new KeyNotFoundException($"FAQ with id {askedQuestionId} was not found.");
            }

            return frequentlyAskedQuestion;
        }

    public int CreateNewEntity(FAQEntry questionEntity)
    {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

            SqlCommand insetEntryCommand = new SqlCommand(
                "INSERT INTO FAQEntry (question, answer, category) " +
                "OUTPUT INSERTED.FAQentry_id " +
                "VALUES (@question, @answer, @category)");

            insetEntryCommand.Parameters.AddWithValue("@question", questionEntity.Question);
            insetEntryCommand.Parameters.AddWithValue("@answer", questionEntity.Answer);
            insetEntryCommand.Parameters.AddWithValue("@category", questionEntity.Category.ToString());

            int addedEntityId = Add(insetEntryCommand, questionEntity);
            InvalidateCacheEntry(addedEntityId);
            return addedEntityId;
        }

    public void UpdateById(int identificationNumber, FAQEntry questionEntity)
    {
            if (questionEntity == null)
            {
                throw new ArgumentNullException(nameof(questionEntity), "FAQ entry cannot be null.");
            }

        SqlCommand updateByIdCommand = new SqlCommand(
            "UPDATE FAQEntry " +
            "SET question = @question, " +
            "answer = @answer, " +
            "category = @category, " +
            "view_count = @viewCount, " +
            "was_helpful_votes = @wasHelpfulVotes, " +
            "was_not_helpful_votes = @wasNotHelpfulVotes " +
            "WHERE FAQentry_id = @id");

        updateByIdCommand.Parameters.AddWithValue("@id", identificationNumber);
        updateByIdCommand.Parameters.AddWithValue("@question", questionEntity.Question);
        updateByIdCommand.Parameters.AddWithValue("@answer", questionEntity.Answer);
        updateByIdCommand.Parameters.AddWithValue("@category", questionEntity.Category.ToString());
        updateByIdCommand.Parameters.AddWithValue("@viewCount", questionEntity.ViewCount);
        updateByIdCommand.Parameters.AddWithValue("@wasHelpfulVotes", questionEntity.HelpfulVotesCount);
        updateByIdCommand.Parameters.AddWithValue("@wasNotHelpfulVotes", questionEntity.NotHelpfulVotesCount);

        UpdateById(identificationNumber, updateByIdCommand, questionEntity);
        InvalidateCacheEntry(identificationNumber);
    }

    public void DeleteById(int identificationNumber)
    {
            SqlCommand deleteByIdCommand = new SqlCommand("DELETE FROM FAQEntry WHERE FAQentry_id = @id");
            deleteByIdCommand.Parameters.AddWithValue("@id", identificationNumber);

            DeleteById(identificationNumber, deleteByIdCommand);
    }

    public IEnumerable<FAQEntry> GetAll()
    {
        SqlCommand getAllCommand = new SqlCommand("SELECT * FROM FAQEntry");
        return GetAll(getAllCommand);
    }

    public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
    {
            SqlCommand getByCategoryCommand;

            if (category == FAQCategoryEnum.All)
            {
                getByCategoryCommand = new SqlCommand("SELECT * FROM FAQEntry");
            }
            else
            {
                getByCategoryCommand = new SqlCommand("SELECT * FROM FAQEntry WHERE category = @category");
                getByCategoryCommand.Parameters.AddWithValue("@category", category.ToString());
            }

            return GetAll(getByCategoryCommand).ToList();
    }

    public void IncrementViewCount(int identificationNumber)
    {
            SqlCommand updateViewCountCommand = new SqlCommand(
                "UPDATE FAQEntry SET view_count = view_count + 1 WHERE FAQentry_id = @id");
            updateViewCountCommand.Parameters.AddWithValue("@id", identificationNumber);

            ExecuteNonQuery(updateViewCountCommand);
            InvalidateCacheEntry(identificationNumber);
        }

    public void IncrementWasHelpfulVotes(int identificationNumber)
    {
            SqlCommand updateWasHelpfulVotesCommand = new SqlCommand(
                "UPDATE FAQEntry SET was_helpful_votes = was_helpful_votes + 1 WHERE FAQentry_id = @id");
            updateWasHelpfulVotesCommand.Parameters.AddWithValue("@id", identificationNumber);

            ExecuteNonQuery(updateWasHelpfulVotesCommand);
            InvalidateCacheEntry(identificationNumber);
        }

    public void IncrementWasNotHelpfulVotes(int identificationNumber)
    {
            SqlCommand updateWasNotHelpfulVotesCommand = new SqlCommand(
                     "UPDATE FAQEntry SET was_not_helpful_votes = was_not_helpful_votes + 1 WHERE FAQentry_id = @id");
            updateWasNotHelpfulVotesCommand.Parameters.AddWithValue("@id", identificationNumber);

            ExecuteNonQuery(updateWasNotHelpfulVotesCommand);
            InvalidateCacheEntry(identificationNumber);
        }

    protected override FAQEntry MapRowToEntity(SqlDataReader reader)
        {
            int identificationNumber = (int)reader["FAQentry_id"];
            string question = reader["question"].ToString();
            string answer = reader["answer"].ToString();
            FAQCategoryEnum category = Enum.Parse<FAQCategoryEnum>(reader["category"].ToString());
            int viewCount = (int)reader["view_count"];
            int helpfulVotesCount = (int)reader["was_helpful_votes"];
            int notHelpfulVotesCount = (int)reader["was_not_helpful_votes"];

            return new FAQEntry(identificationNumber, question, answer, category, viewCount, helpfulVotesCount, notHelpfulVotesCount);
        }

    protected override int GetEntityId(FAQEntry questionEntity)
    {
            return questionEntity.Id;
    }
}
}