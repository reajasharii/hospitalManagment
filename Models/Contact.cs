namespace HospitalManagement.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string ReasonForContact { get; set; }
        public required string Description { get; set; }
    }
}
