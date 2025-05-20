namespace lab6.Models
{
    public class OffenseType
    {
        public int OffenseTypeID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty; 
        }
    }
}