namespace HospitalManagement.ViewModels
{
    public class FeedbackViewModel
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string DoctorFullName { get; set; }
        public string PatientFullName { get; set; }
        public string FeedbackText { get; set; }
    }
}
