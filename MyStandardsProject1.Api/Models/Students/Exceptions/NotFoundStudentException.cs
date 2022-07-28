using System;
using Xeptions;

namespace MyStandardsProject1.Api.Models.Students.Exceptions
{
    public class NotFoundStudentException : Xeption
    {
        public NotFoundStudentException(Guid studentId)
            : base(message: $"Couldn't find student with studentId: {studentId}.")
        { }
    }
}