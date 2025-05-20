using System;

namespace lab6.Models
{
    public class Investigation
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