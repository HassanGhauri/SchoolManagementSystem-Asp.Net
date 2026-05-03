using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Api.Models
{
    public class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClassName { get; set; } = string.Empty;

        // Class Teacher (1 teacher per class)
        public int? ClassTeacherId { get; set; }
        public User? ClassTeacher { get; set; }

        // Many-to-Many Relationships
        public ICollection<ClassStudent> Students { get; set; } = new List<ClassStudent>();

        public ICollection<ClassTeacher> AssignedTeachers { get; set; } = new List<ClassTeacher>();

        public ICollection<ClassSubject> Subjects { get; set; } = new List<ClassSubject>();
    }
}