namespace SchoolManagementSystem.Api.DTOs
{
    public class AIResponse
    {
        public string Intent { get; set; } = string.Empty;

        public string? ClassName { get; set; }

        public string? SubjectName { get; set; }

        public string? TeacherName { get; set; }

        // NEW
        public int? AgeLessThan { get; set; }

        public int? AgeGreaterThan { get; set; }
    }
}