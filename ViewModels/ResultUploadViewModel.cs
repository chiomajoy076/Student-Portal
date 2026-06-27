using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class ResultUploadViewModel
{
    [Required]
    [Display(Name = "Matric Number")]
    public string MatricNumber { get; set; }

    [Required]
    [Display(Name = "Course")]
    public int CourseId { get; set; }

    [Required]
    [Range(0, 100)]
    public int Score { get; set; }
}
