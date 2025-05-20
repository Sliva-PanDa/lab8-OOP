using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using lab6.Models;

namespace lab6.Repositories
{
    public class SubjectRepository : IRepository<Subject>
    {
        private readonly string _connectionString;

        public SubjectRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public IEnumerable<Subject> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT SubjectID, LastName, FirstName, Notes FROM Subjects ORDER BY LastName, FirstName";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            yield return new Subject
                            {
                                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                            };
                        }
                    }
                }
            }
        }

        public Subject GetById(int id)
        {
            Subject subject = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT SubjectID, LastName, FirstName, Notes FROM Subjects WHERE SubjectID = @SubjectID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            subject = new Subject
                            {
                                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                            };
                        }
                    }
                }
            }
            return subject;
        }

        public void Add(Subject entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность Subject не может быть null.");
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new ArgumentException("Фамилия субъекта не может быть пустой.", nameof(entity.LastName));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "INSERT INTO Subjects (LastName, FirstName, Notes) VALUES (@LastName, @FirstName, @Notes)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@LastName", entity.LastName);
                    command.Parameters.AddWithValue("@FirstName", (object)entity.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)entity.Notes ?? DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Subject entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность Subject не может быть null.");
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new ArgumentException("Фамилия субъекта не может быть пустой для обновления.", nameof(entity.LastName));
            if (entity.SubjectID <= 0)
                throw new ArgumentException("Некорректный ID для обновления Subject.", nameof(entity.SubjectID));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "UPDATE Subjects SET LastName = @LastName, FirstName = @FirstName, Notes = @Notes WHERE SubjectID = @SubjectID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@LastName", entity.LastName);
                    command.Parameters.AddWithValue("@FirstName", (object)entity.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)entity.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SubjectID", entity.SubjectID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный ID для удаления Subject.", nameof(id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "DELETE FROM Subjects WHERE SubjectID = @SubjectID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}