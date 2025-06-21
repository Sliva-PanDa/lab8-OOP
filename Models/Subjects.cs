using System;
using System.Data.Linq.Mapping;

namespace InvestigationApp.Models
{
    public class Subjects
    {
        public int SubjectID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Notes { get; set; }
    }
}