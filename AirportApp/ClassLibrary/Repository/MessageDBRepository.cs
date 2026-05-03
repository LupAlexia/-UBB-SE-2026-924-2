using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public class MessageDatabaseRepository : DatabaseRepository<int, Message>, IRepository<int, Message>
    {
        protected override Message MapRowToEntity(SqlDataReader reader)
        {
            int messageId = reader.GetInt32(reader.GetOrdinal("message_id"));
            int chatId = reader.GetInt32(reader.GetOrdinal("chat_id"));
            int senderId = reader.GetInt32(reader.GetOrdinal("sender_id"));
            string text = reader.GetString(reader.GetOrdinal("text"));
            /// DateTimeOffset timestamp = reader.GetDateTimeOffset(reader.GetOrdinal("timestamp"));
            // Read as DateTime, then convert to DateTimeOffset
            DateTime databaseDateTime = reader.GetDateTime(reader.GetOrdinal("timestamp"));
            DateTimeOffset timestamp = new DateTimeOffset(databaseDateTime);

            var senderPlaceholder = new SenderStub(senderId);
            var chatPlaceholder = new ChatStub(chatId);

            return new Message(messageId, senderPlaceholder, chatPlaceholder, text, timestamp);
        }

        protected override int GetEntityId(Message entity) => entity.GetId();

        public int CreateNewEntity(Message newEntity)
        {
            const string insertQuery =
                "INSERT INTO Message (sender_id, chat_id, timestamp, text, is_read) " +
                "VALUES (@senderId, @chatId, @timestamp, @text, @isRead); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var insertCommand = new SqlCommand(insertQuery);
            insertCommand.Parameters.AddWithValue("@senderId", newEntity.GetSender().RetrieveUniqueDatabaseIdentifierForBot());
            insertCommand.Parameters.AddWithValue("@chatId", ((IMessage)newEntity).GetChat().Id);
            insertCommand.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow);
            insertCommand.Parameters.AddWithValue("@text", newEntity.GetMessage());
            insertCommand.Parameters.AddWithValue("@isRead", false);

            return Add(insertCommand, newEntity);
        }

        public void DeleteById(int identificationNumber)
        {
            const string deleteQuery = "DELETE FROM Message WHERE message_id = @id";
            var deleteCommand = new SqlCommand(deleteQuery);
            deleteCommand.Parameters.AddWithValue("@id", identificationNumber);

            DeleteById(identificationNumber, deleteCommand);
        }

        public void UpdateById(int identificationNumber, Message message)
        {
            const string updateQuery = "UPDATE Message SET text = @text WHERE message_id = @id";
            var updateCommand = new SqlCommand(updateQuery);
            updateCommand.Parameters.AddWithValue("@text", message.GetMessage());
            updateCommand.Parameters.AddWithValue("@id", identificationNumber);

            UpdateById(identificationNumber, updateCommand, message);
        }

        public IEnumerable<Message> GetAll()
        {
            const string selectQuery = "SELECT * FROM Message";
            return GetAll(new SqlCommand(selectQuery));
        }

        public Message GetById(int identificationNumber)
        {
            const string selectByIQuery = "SELECT * FROM Message WHERE message_id = @id";
            var selectCommand = new SqlCommand(selectByIQuery);
            selectCommand.Parameters.AddWithValue("@id", identificationNumber);

            return GetById(identificationNumber, selectCommand)
                ?? throw new KeyNotFoundException($"Message with id {identificationNumber} not found.");
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            const string selectQuery =
                "SELECT * FROM Message WHERE chat_id = @chatId ORDER BY timestamp ASC";
            var selectCommand = new SqlCommand(selectQuery);
            selectCommand.Parameters.AddWithValue("@chatId", chatId);

            return GetAll(selectCommand);
        }

        public IEnumerable<Message> GetMessagesSince(int chatId, int firstMessageId)
        {
            const string selectMessageQuery =
                "SELECT * FROM Message " +
                "WHERE chat_id = @chatId AND message_id >= @firstMessageId " +
                "ORDER BY timestamp ASC";
            var selectCommand = new SqlCommand(selectMessageQuery);
            selectCommand.Parameters.AddWithValue("@chatId", chatId);
            selectCommand.Parameters.AddWithValue("@firstMessageId", firstMessageId);

            return GetAll(selectCommand);
        }

        public void MarkAsRead(int messageId)
        {
            const string updateMessageQuery = "UPDATE Message SET is_read = 1 WHERE message_id = @id";
            var updateCommand = new SqlCommand(updateMessageQuery);
            updateCommand.Parameters.AddWithValue("@id", messageId);

            ExecuteNonQuery(updateCommand);
            InvalidateCacheEntry(messageId);
        }

        // TODO: I swear I wanted to remove stubs, not end more. I hope God and Mihai will forgive me, at least Mihai.
        private sealed class SenderStub : ISender
        {
            private readonly int identificationNumber;
            public SenderStub(int identificationNumber) => this.identificationNumber = identificationNumber;
            public int RetrieveUniqueDatabaseIdentifierForBot() => identificationNumber;
            public string RetrieveConfiguredDisplayFullNameForBot() => string.Empty;
            public string RetrieveConfiguredEmailAddressForBotContact() => string.Empty;
        }
       private sealed class ChatStub : Chat
        {
            public ChatStub(int chatId) : base(chatId, userId: 0, ChatStatus.Active)
            {
            }
        }
    }
}