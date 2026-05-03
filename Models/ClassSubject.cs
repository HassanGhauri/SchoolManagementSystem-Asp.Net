using System;

namespace SchoolManagementSystem.Api.Models;

public class ClassSubject
{
    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}
