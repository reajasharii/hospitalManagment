namespace HospitalManagement.Models
{
public class Departments
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public required string PhoneNumber { get; set; }
}

}