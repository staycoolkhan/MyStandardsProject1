using System;
using MyStandardsProject1.Api.Models.Students;
using MyStandardsProject1.Api.Models.Students.Exceptions;

namespace MyStandardsProject1.Api.Services.Foundations.Students
{
    public partial class StudentService
    {
        private void ValidateStudentOnAdd(Student student)
        {
            ValidateStudentIsNotNull(student);

            Validate(
                (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                (Rule: IsInvalid(student.CreatedByUserId), Parameter: nameof(Student.CreatedByUserId)),
                (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),
                (Rule: IsInvalid(student.UpdatedByUserId), Parameter: nameof(Student.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: student.UpdatedDate,
                    secondDate: student.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),
                Parameter: nameof(Student.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: student.UpdatedByUserId,
                    secondId: student.CreatedByUserId,
                    secondIdName: nameof(Student.CreatedByUserId)),
                Parameter: nameof(Student.UpdatedByUserId)));
        }

        private static void ValidateStudentIsNotNull(Student student)
        {
            if (student is null)
            {
                throw new NullStudentException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidStudentException = new InvalidStudentException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidStudentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidStudentException.ThrowIfContainsErrors();
        }
    }
}