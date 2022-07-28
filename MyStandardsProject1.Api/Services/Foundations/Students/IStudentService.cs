using System;
using System.Linq;
using System.Threading.Tasks;
using MyStandardsProject1.Api.Models.Students;

namespace MyStandardsProject1.Api.Services.Foundations.Students
{
    public interface IStudentService
    {
        ValueTask<Student> AddStudentAsync(Student student);
        IQueryable<Student> RetrieveAllStudents();
        ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId);
        ValueTask<Student> ModifyStudentAsync(Student student);
    }
}