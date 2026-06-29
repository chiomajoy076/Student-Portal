using System.ComponentModel.DataAnnotations;

namespace Student_Portal.ViewModels;

public class ResultRowInput
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    [Range(0, 100)]
    public int Score { get; set; }
}

public class BulkResultUploadViewModel
{
    [Required]
    [Display(Name = "Matric Number")]
    public string MatricNumber { get; set; }

    public List<ResultRowInput> Rows { get; set; } = new();
}
