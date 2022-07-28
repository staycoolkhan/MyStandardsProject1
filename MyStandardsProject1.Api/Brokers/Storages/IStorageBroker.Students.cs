using System;
using System.Linq;
using System.Threading.Tasks;
using MyStandardsProject1.Api.Models.Students;

namespace MyStandardsProject1.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Student> InsertStudentAsync(Student student);
        IQueryable<Student> SelectAllStudents();
        ValueTask<Student> SelectStudentByIdAsync(Guid studentId);
    }
}
