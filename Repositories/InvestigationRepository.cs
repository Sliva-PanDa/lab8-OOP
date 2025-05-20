using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using lab6.Models;

namespace lab6.Repositories
{
    public class InvestigationRepository : IRepository<Investigation>
    {
        private readonly string _connectionString;

        public InvestigationRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private Investigation MapInvestigationFromReader(SqlDataReader dataReader)
        {
            return new Investigation
            {
                InvestigationID = dataReader.GetInt32(dataReader.GetOrdinal("InvestigationID")),
                SubjectID = dataReader.GetInt32(dataReader.GetOrdinal("SubjectID")),
                OffenseTypeID = dataReader.GetInt32(dataReader.GetOrdinal("OffenseTypeID")),
                OffenseDate = dataReader.GetDateTime(dataReader.GetOrdinal("OffenseDate")),
                RewardAmount = dataReader.GetDecimal(dataReader.GetOrdinal("RewardAmount")),
                Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? null : dataReader.GetString(dataReader.GetOrdinal("Description")),
                Status = dataReader.IsDBNull(dataReader.GetOrdinal("Status")) ? null : dataReader.GetString(dataReader.GetOrdinal("Status"))
            };
        }

        public IEnumerable<Investigation> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT InvestigationID, SubjectID, OffenseTypeID,
                                           OffenseDate, RewardAmount, Description, Status
                                    FROM Investigations
                                    ORDER BY OffenseDate DESC, InvestigationID DESC";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            yield return MapInvestigationFromReader(reader);
                        }
                    }
                }
            }
        }

        public Investigation GetById(int id)
        {
            Investigation investigation = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT InvestigationID, SubjectID, OffenseTypeID,
                                           OffenseDate, RewardAmount, Description, Status
                                    FROM Investigations
                                    WHERE InvestigationID = @InvestigationID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@InvestigationID", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            investigation = MapInvestigationFromReader(reader);
                        }
                    }
                }
            }
            return investigation;
        }

        public void Add(Investigation entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность Investigation не может быть null.");
            if (entity.SubjectID <= 0)
                throw new ArgumentException("Некорректный SubjectID.", nameof(entity.SubjectID));
            if (entity.OffenseTypeID <= 0)
                throw new ArgumentException("Некорректный OffenseTypeID.", nameof(entity.OffenseTypeID));
            if (entity.OffenseDate == DateTime.MinValue)
                throw new ArgumentException("Дата правонарушения должна быть указана.", nameof(entity.OffenseDate));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT INTO Investigations (SubjectID, OffenseTypeID, OffenseDate,
                                                           RewardAmount, Description, Status)
                                    VALUES (@SubjectID, @OffenseTypeID, @OffenseDate,
                                            @RewardAmount, @Description, @Status)";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", entity.SubjectID);
                    command.Parameters.AddWithValue("@OffenseTypeID", entity.OffenseTypeID);
                    command.Parameters.AddWithValue("@OffenseDate", entity.OffenseDate);
                    command.Parameters.AddWithValue("@RewardAmount", entity.RewardAmount);
                    command.Parameters.AddWithValue("@Description", (object)entity.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", (object)entity.Status ?? DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Investigation entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Сущность Investigation не может быть null.");
            if (entity.InvestigationID <= 0)
                throw new ArgumentException("Некорректный ID для обновления Investigation.", nameof(entity.InvestigationID));
            if (entity.SubjectID <= 0)
                throw new ArgumentException("Некорректный SubjectID.", nameof(entity.SubjectID));
            if (entity.OffenseTypeID <= 0)
                throw new ArgumentException("Некорректный OffenseTypeID.", nameof(entity.OffenseTypeID));
            if (entity.OffenseDate == DateTime.MinValue)
                throw new ArgumentException("Дата правонарушения должна быть указана.", nameof(entity.OffenseDate));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE Investigations
                                    SET SubjectID = @SubjectID,
                                        OffenseTypeID = @OffenseTypeID,
                                        OffenseDate = @OffenseDate,
                                        RewardAmount = @RewardAmount,
                                        Description = @Description,
                                        Status = @Status
                                    WHERE InvestigationID = @InvestigationID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", entity.SubjectID);
                    command.Parameters.AddWithValue("@OffenseTypeID", entity.OffenseTypeID);
                    command.Parameters.AddWithValue("@OffenseDate", entity.OffenseDate);
                    command.Parameters.AddWithValue("@RewardAmount", entity.RewardAmount);
                    command.Parameters.AddWithValue("@Description", (object)entity.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", (object)entity.Status ?? DBNull.Value);
                    command.Parameters.AddWithValue("@InvestigationID", entity.InvestigationID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный ID для удаления Investigation.", nameof(id));
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "DELETE FROM Investigations WHERE InvestigationID = @InvestigationID";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@InvestigationID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}