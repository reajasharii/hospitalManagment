namespace HospitalManagement.Models
{
    public class AboutUsContent
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public System.DateTime CreatedAt { get; set; } = System.DateTime.Now;
        public System.DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
