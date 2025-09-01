using Student_Portal.Models;
using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class StudentFormViewModel
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Matric Number")]
    public string MatricNumber { get; set; }
    
    [Required]
    public string Department { get; set; }
    
    [Required]
    public string Level { get; set; }
    
    public string? ProfileImageBase64 { get; set; }
    public string? UploadedDocumentBase64 { get; set; }
    public bool IsSubmitted { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public DateTime? SubmittedAt { get; set; }

    [Display(Name = "Jamb Registration Number")]
    public string JambRegNumber { get; set; }
    [Display(Name = "WAEC Registration Number")]
    public string WAECRegNumber { get; set; }
    [Display(Name = "Mode of Entry")]
    public ModeOfEntry? ModeOfEntry { get; set; }
    [Display(Name = "Jamb Score")]
    public int JambScore { get; set; }
    [Display(Name = "Post Utme Score")]
    public int PostUtmeScore { get; set; }
    [Display(Name = "Marital Status")]
    public MaritalStatus? MaritalStatus { get; set; }
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; }
    public string State { get; set; }
    [Display(Name = "Local Government Area")]
    public string LocalGov { get; set; }
    [Display(Name = "Next of Kin")]
    public string NextOfKin { get; set; }
    [Display(Name = "Next of Kin Phone Number")]
    public string NextOfKinPhoneNumber { get; set; }
}