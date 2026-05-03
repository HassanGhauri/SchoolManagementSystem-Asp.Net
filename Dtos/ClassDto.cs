namespace SchoolManagementSystem.Api.DTOs
{
    public class ClassDto
{
    public int Id { get; set; }
    public string ClassName { get; set; }

    public string? ClassTeacherName { get; set; }

    public List<UserDto> Students { get; set; } = new();
    public List<string> Subjects { get; set; } = new();
    public List<UserDto> AssignedTeachers { get; set; } = new();
}
}