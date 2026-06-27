using System.ComponentModel.DataAnnotations;
using Student_Portal.Models;

namespace Student_Portal.ViewModels;

public class EditStudentViewModel
{
    public string Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    public Gender? Gender { get; set; }

    public string MatricNumber { get; set; }
    public string Department { get; set; }
    public string Level { get; set; }
}
