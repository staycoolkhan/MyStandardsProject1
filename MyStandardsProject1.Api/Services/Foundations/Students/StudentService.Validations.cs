using MyStandardsProject1.Api.Models.Students;
using MyStandardsProject1.Api.Models.Students.Exceptions;

namespace MyStandardsProject1.Api.Services.Foundations.Students
{
    public partial class StudentService
    {
        private void ValidateStudentOnAdd(Student student)
        {
            ValidateStudentIsNotNull(student);
        }

        private static void ValidateStudentIsNotNull(Student student)
        {
            if (student is null)
            {
                throw new NullStudentException();
            }
        }
    }
}