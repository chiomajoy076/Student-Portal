using System.ComponentModel.DataAnnotations;
using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; }

    [Required]
    [Display(Name = "Course Title")]
    public string CourseTitle { get; set; }

    [Required]
    [Range(1, 10)]
    [Display(Name = "Credit Unit")]
    public int CreditUnit { get; set; }

    [Required]
    public Semester Semester { get; set; }

    [Required]
    public string Session { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    public string Level { get; set; }
}
