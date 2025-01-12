namespace HospitalManagement.Models
{
    public class HospitalStaff
    {
        public int Id { get; set; }
        
        // Përdorimi i emrit (FirstName dhe LastName)
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        
        // Krijimi i FullName
        public string FullName => $"{FirstName} {LastName}";

        // Pozita dhe Departamenti
        public required string Position { get; set; }
        public required string Department { get; set; }
        public required string WorkSchedule { get; set; }

        // Heqja e atributit Contact nga model
    }
}
