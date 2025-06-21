using System;
using System.Data.Linq.Mapping;

namespace InvestigationApp.Models
{
    public class Investigations
    {
        public int InvestigationID { get; set; }

        public int SubjectID { get; set; }

        public int OffenseTypeID { get; set; }

        public DateTime OffenseDate { get; set; }

        public decimal RewardAmount { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}