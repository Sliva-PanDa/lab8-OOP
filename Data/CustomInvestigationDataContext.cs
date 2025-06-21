using System;
using System.Data.Linq;
using System.Data.SqlClient;
using InvestigationApp.Models;

namespace InvestigationApp.Data
{
    public class CustomInvestigationDataContext : DataContext
    {
        private static readonly string ConnectionString =
            "Data Source=DESKTOP-O5LSFQA\\SQLEXPRESS;Initial Catalog=DetectiveDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public CustomInvestigationDataContext() : base(ConnectionString)
        {
        }

        public Table<Investigations> Investigations;
        public Table<OffenseTypes> OffenseTypes;
        public Table<Subjects> Subjects;
    }
}