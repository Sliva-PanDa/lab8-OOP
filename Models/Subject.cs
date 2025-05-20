namespace lab6.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; } 
        public string Notes { get; set; }     

        public string FullName => $"{LastName} {FirstName}".Trim();

        public override string ToString()
        {
            return FullName;
        }
    }
}