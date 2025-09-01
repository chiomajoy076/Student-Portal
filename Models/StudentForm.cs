namespace Student_Portal.Models;

public class StudentForm
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string JambRegNumber { get; set; }
    public string WAECRegNumber { get; set; }
    public ModeOfEntry? ModeOfEntry { get; set; }
    public int JambScore { get; set; }
    public int PostUtmeScore { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; }
    public string State { get; set; }
    public string LocalGov { get; set; }
    public string NextOfKin { get; set; }
    public string NextOfKinPhoneNumber { get; set; }
    public string MatricNumber { get; set; }
    public string Department { get; set; }
    public string Level { get; set; }
    public string? UploadedDocument { get; set; } // Base64 document
    public bool IsSubmitted { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public enum ModeOfEntry
{
    PUTME = 1,
    DE
}