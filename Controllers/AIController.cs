using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Api.Data;
using SchoolManagementSystem.Api.Services;

namespace SchoolManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AISearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly AIService _aiService;

        public AISearchController(
            AppDbContext context,
            AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(
            [FromBody] SearchRequest request)
        {
            // ===================================
            // AI DETECTS INTENT
            // ===================================

            var intent =
                await _aiService.DetectIntent(
                    request.Question
                );

            Console.WriteLine(intent.Intent);

            // ===================================
            // GET STUDENTS BY CLASS
            // ===================================

            if (
                intent.Intent ==
                "get_students_by_class"
            )
            {
                var students =
                    await _context.ClassStudents
                        .Include(x => x.Student)
                        .Include(x => x.Class)
                        .Where(x =>
                            x.Class.ClassName ==
                            intent.ClassName
                        )
                        .Select(x => new
                        {
                            x.Student.Id,
                            x.Student.FirstName,
                            x.Student.LastName,
                            x.Student.Age,
                            x.Class.ClassName
                        })
                        .ToListAsync();

                return Ok(students);
            }

            // ===================================
            // GET TEACHERS BY CLASS
            // ===================================

            if (
                intent.Intent ==
                "get_teachers_by_class"
            )
            {
                var teachers =
                    await _context.ClassTeachers
                        .Include(x => x.Teacher)
                        .Include(x => x.Class)
                        .Where(x =>
                            x.Class.ClassName ==
                            intent.ClassName
                        )
                        .Select(x => new
                        {
                            x.Teacher.Id,
                            x.Teacher.FirstName,
                            x.Teacher.LastName,
                            x.Teacher.Age,
                            x.Class.ClassName
                        })
                        .ToListAsync();

                return Ok(teachers);
            }

            // ===================================
            // GET SUBJECTS BY CLASS
            // ===================================

            if (
                intent.Intent ==
                "get_subjects_by_class"
            )
            {
                var subjects =
                    await _context.ClassSubjects
                        .Include(x => x.Subject)
                        .Include(x => x.Class)
                        .Where(x =>
                            x.Class.ClassName ==
                            intent.ClassName
                        )
                        .Select(x => new
                        {
                            x.Subject.Id,
                            x.Subject.Name,
                            x.Class.ClassName
                        })
                        .ToListAsync();

                return Ok(subjects);
            }

            // ===================================
            // GET STUDENTS BY AGE
            // ===================================

            if (
                intent.Intent ==
                "get_students_by_age"
            )
            {
                var query = _context.Users
                    .Where(x => x.Role == "Student")
                    .AsQueryable();

                // AGE LESS THAN

                if (intent.AgeLessThan.HasValue)
                {
                    query = query.Where(x =>
                        x.Age <
                        intent.AgeLessThan.Value
                    );
                }

                // AGE GREATER THAN

                if (intent.AgeGreaterThan.HasValue)
                {
                    query = query.Where(x =>
                        x.Age >
                        intent.AgeGreaterThan.Value
                    );
                }

                var students = await query
                    .Select(x => new
                    {
                        x.Id,
                        x.FirstName,
                        x.LastName,
                        x.Age
                    })
                    .ToListAsync();

                return Ok(students);
            }

            // ===================================
            // UNKNOWN INTENT
            // ===================================

            return BadRequest(new
            {
                error = "Unknown query intent"
            });
        }
    }

    public class SearchRequest
    {
        public string Question { get; set; } = string.Empty;
    }
}