namespace HospitalManagement.Models
{
    public class HospitalStaff
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Position { get; set; }
        public required string Department { get; set; }
        public required string WorkingHours { get; set; }
    }
}
