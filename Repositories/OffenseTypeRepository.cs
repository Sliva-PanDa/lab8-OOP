using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using lab6.Models;

namespace lab6.Repositories
{
    public class OffenseTypeRepository : IRepository<OffenseType>
    {
        private readonly string _connectionString;

        public OffenseTypeRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public IEnumerable<OffenseType> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT OffenseTypeID, Name FROM OffenseTypes ORDER BY Name";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            yield return new OffenseType
                            {
                                OffenseTypeID = reader.GetInt32(reader.GetOrdinal("OffenseTypeID")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                    }
                }
            }
        }

        public OffenseType GetById(int id)
        {
            OffenseType offenseType = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT OffenseTypeID, Name FROM OffenseTypes WHERE OffenseTypeID = @OffenseTypeID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@OffenseTypeID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            offenseType = new OffenseType
                            {
                                OffenseTypeID = reader.GetInt32(reader.GetOrdinal("OffenseTypeID")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                    }
                }
            }
            return offenseType;
        }

        public void Add(OffenseType entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность OffenseType не может быть null.");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Имя типа правонарушения не может быть пустым.", nameof(entity.Name));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "INSERT INTO OffenseTypes (Name) VALUES (@Name)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(OffenseType entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность OffenseType не может быть null.");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Имя типа правонарушения не может быть пустым для обновления.", nameof(entity.Name));
            if (entity.OffenseTypeID <= 0)
                throw new ArgumentException("Некорректный ID для обновления OffenseType.", nameof(entity.OffenseTypeID));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "UPDATE OffenseTypes SET Name = @Name WHERE OffenseTypeID = @OffenseTypeID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@OffenseTypeID", entity.OffenseTypeID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный ID для удаления OffenseType.", nameof(id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "DELETE FROM OffenseTypes WHERE OffenseTypeID = @OffenseTypeID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@OffenseTypeID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}