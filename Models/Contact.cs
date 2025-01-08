namespace HospitalManagement.Models
{
    public class Contact
    {
        public int Id { get; set; } // ID unike për çdo kontakt

        public required string FirstName { get; set; } // Emri i parë
        public required string LastName { get; set; } // Mbiemri
        public required string Email { get; set; } // Email-i i përdoruesit
        public required string PhoneNumber { get; set; } // Numri i telefonit
        public required string ReasonForContact { get; set; } // Arsyeja për kontaktim
        public required string Description { get; set; } // Përshkrimi (mesazhi)
    }
}
