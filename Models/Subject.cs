using System;

namespace SchoolManagementSystem.Api.Models;

public class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<ClassSubject> Classes { get; set; } = new List<ClassSubject>();
}
