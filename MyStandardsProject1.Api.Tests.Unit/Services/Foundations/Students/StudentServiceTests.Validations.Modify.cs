using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using MyStandardsProject1.Api.Models.Students;
using MyStandardsProject1.Api.Models.Students.Exceptions;
using Xunit;

namespace MyStandardsProject1.Api.Tests.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentIsNullAndLogItAsync()
        {
            // given
            Student nullStudent = null;
            var nullStudentException = new NullStudentException();

            var expectedStudentValidationException =
                new StudentValidationException(nullStudentException);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(nullStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            // then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStudentAsync(It.IsAny<Student>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidStudent = new Student
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            //invalidStudentException.AddData(
            //    key: nameof(Student.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Student model

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: "Date is required");

            invalidStudentException.AddData(
                key: nameof(Student.CreatedByUserId),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Student.CreatedDate)}"
                });

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedByUserId),
                values: "Id is required");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            //then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStudentAsync(It.IsAny<Student>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomStudent(randomDateTimeOffset);
            Student invalidStudent = randomStudent;
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: $"Date is the same as {nameof(Student.CreatedDate)}");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            // then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(invalidStudent.Id),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomStudent(randomDateTimeOffset);
            randomStudent.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidStudentException =
                new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: "Date is not recent");

            var expectedStudentValidatonException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(randomStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            // then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomModifyStudent(randomDateTimeOffset);
            Student nonExistStudent = randomStudent;
            Student nullStudent = null;

            var notFoundStudentException =
                new NotFoundStudentException(nonExistStudent.Id);

            var expectedStudentValidationException =
                new StudentValidationException(notFoundStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(nonExistStudent.Id))
                .ReturnsAsync(nullStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(nonExistStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            // then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(nonExistStudent.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Student randomStudent = CreateRandomModifyStudent(randomDateTimeOffset);
            Student invalidStudent = randomStudent.DeepClone();
            Student storageStudent = invalidStudent.DeepClone();
            storageStudent.CreatedDate = storageStudent.CreatedDate.AddMinutes(randomMinutes);
            storageStudent.UpdatedDate = storageStudent.UpdatedDate.AddMinutes(randomMinutes);
            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: $"Date is not the same as {nameof(Student.CreatedDate)}");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStudentByIdAsync(invalidStudent.Id))
                .ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    modifyStudentTask.AsTask);

            // then
            actualStudentValidationException.Should()
                .BeEquivalentTo(expectedStudentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStudentByIdAsync(invalidStudent.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedStudentValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}