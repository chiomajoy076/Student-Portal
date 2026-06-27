using Student_Portal.Models;

namespace Student_Portal.Services;

public static class GradeCalculator
{
    public static Grade FromScore(int score) => score switch
    {
        >= 70 => Grade.A,
        >= 60 => Grade.B,
        >= 50 => Grade.C,
        >= 45 => Grade.D,
        >= 40 => Grade.E,
        _ => Grade.F
    };
}
