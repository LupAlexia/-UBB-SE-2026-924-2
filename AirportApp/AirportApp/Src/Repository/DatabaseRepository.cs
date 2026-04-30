using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Database;
using Microsoft.Data.SqlClient;
namespace AirportApp.Src.Repository.Database
{
    public abstract class DatabaseRepository<TKey, TEntity>
        where TEntity : class
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TEntity> entityCache = new ();

        protected SqlConnection CreateConnection() => DatabaseConnectionHandler.Instance.CreateConnection();
        protected abstract TEntity MapRowToEntity(SqlDataReader reader);
        protected abstract TKey GetEntityId(TEntity entity);

        /// <summary>
        /// Gets an entity by its id OR returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected TEntity GetById(TKey id, SqlCommand command)
        {
            if (entityCache.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var entity = ExecuteQuerySingle(command);
            if (entity != null)
            {
                entityCache[id] = entity;
            }

            return entity;
        }

        protected IEnumerable<TEntity> GetAll(SqlCommand executeCommand)
        {
            var results = ExecuteQueryMany(executeCommand).ToList();
            foreach (var entity in results)
            {
                entityCache[GetEntityId(entity)] = entity;
            }
            return results;
        }

        protected TKey Add(SqlCommand command, TEntity entity)
        {
            var identificationNumber = ExecuteScalar<TKey>(command);
            entityCache[identificationNumber] = entity;
            return identificationNumber;
        }

        protected void DeleteById(TKey identificationNUMBER, SqlCommand executeCommand)
        {
            ExecuteNonQuery(executeCommand);
            entityCache.Remove(identificationNUMBER);
        }

        protected void UpdateById(TKey identificationNumber, SqlCommand executeCommand, TEntity entity)
        {
            ExecuteNonQuery(executeCommand);
            entityCache[identificationNumber] = entity;
        }

        // NOTE : If testing becomes a requirement, override the following query methods to work over something in memory.

        /// <summary>
        /// Returns one entity matching the query. If no matching row in db is found => null!
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual TEntity ExecuteQuerySingle(SqlCommand command)
        {
            using var connection = CreateConnection();
            command.Connection = connection;
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapRowToEntity(reader) : null;
        }

        protected virtual IEnumerable<TEntity> ExecuteQueryMany(SqlCommand executeCommand)
        {
            using var connection = CreateConnection();
            executeCommand.Connection = connection;
            connection.Open();
            using var reader = executeCommand.ExecuteReader();
            var results = new List<TEntity>();
            while (reader.Read())
            {
                results.Add(MapRowToEntity(reader));
            }
            return results;
        }

        protected virtual void ExecuteNonQuery(SqlCommand executeCommand)
        {
            using var connection = CreateConnection();
            executeCommand.Connection = connection;
            connection.Open();
            executeCommand.ExecuteNonQuery();
        }

        protected virtual T ExecuteScalar<T>(SqlCommand executeCommand)
        {
            using var connection = CreateConnection();
            executeCommand.Connection = connection;
            connection.Open();
            return (T)executeCommand.ExecuteScalar();
        }
        protected void InvalidateCache() => entityCache.Clear();
        protected void InvalidateCacheEntry(TKey identificationNumber) => entityCache.Remove(identificationNumber);
    }
}