using System;

public class NoteViewModel
{
   
    public string PatientId { get; set; }
    public string Content { get; set; }
    public DateTime? CreatedAt { get; set; } // Optional for listing
}
