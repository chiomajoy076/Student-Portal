using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Student_Portal.Repositories;

namespace Student_Portal.Services;

public class RegistrationSlipService : IRegistrationSlipService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    public RegistrationSlipService(IUserRepository userRepository, IStudentRepository studentRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
    }

    public async Task<byte[]?> GenerateAsync(string userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        var form = await _studentRepository.GetByUserIdAsync(userId);

        if (user == null || form == null || !form.IsSubmitted || !user.IsActive)
        {
            return null;
        }

        var fullName = $"{user.FirstName} {(string.IsNullOrWhiteSpace(user.MiddleName) ? "" : user.MiddleName + " ")}{user.LastName}".Trim();
        var registrationDate = form.SubmittedAt ?? form.LastUpdated;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text("AE-FUNAI").Bold().FontSize(18);
                    column.Item().AlignCenter().Text("Student Registration Slip").FontSize(14);
                    column.Item().PaddingTop(5).LineHorizontal(1);
                });

                page.Content().PaddingTop(15).Column(column =>
                {
                    column.Spacing(8);

                    void Row(string label, string value)
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(150).Text(label).Bold();
                            row.RelativeItem().Text(value);
                        });
                    }

                    Row("Student Name:", fullName);
                    Row("Matric Number:", form.MatricNumber);
                    Row("Registration Number:", form.JambRegNumber);
                    Row("Department:", form.Department);
                    Row("Level:", form.Level);
                    Row("Registration Date:", registrationDate.ToString("MMMM dd, yyyy"));
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("This slip confirms successful registration on the AE-FUNAI Online Registration System.")
                        .FontSize(8).Italic();
                });
            });
        });

        return document.GeneratePdf();
    }
}
