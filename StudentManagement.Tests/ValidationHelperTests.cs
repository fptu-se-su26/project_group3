using StudentManagement.Business.Validators;
using System;
using Xunit;

namespace StudentManagement.Tests
{
    public class ValidationHelperTests
    {
        [Theory]
        [InlineData("student@example.com", true)]
        [InlineData("invalid-email", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidEmail_ChecksFormat(string? email, bool expected)
        {
            Assert.Equal(expected, ValidationHelper.IsValidEmail(email));
        }

        [Fact]
        public void IsPastOrToday_FutureDate_ReturnsFalse()
        {
            Assert.False(ValidationHelper.IsPastOrToday(DateTime.Now.AddDays(1)));
        }

        [Fact]
        public void IsPastOrToday_PastDate_ReturnsTrue()
        {
            Assert.True(ValidationHelper.IsPastOrToday(DateTime.Now.AddYears(-20)));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(10, true)]
        [InlineData(-1, false)]
        [InlineData(10.1, false)]
        public void IsInRange_ChecksScoreBounds(double value, bool expected)
        {
            Assert.Equal(expected, ValidationHelper.IsInRange(value, 0, 10));
        }

        [Theory]
        [InlineData(3, true)]
        [InlineData(0, false)]
        [InlineData(-2, false)]
        public void IsPositive_ChecksCredits(int value, bool expected)
        {
            Assert.Equal(expected, ValidationHelper.IsPositive(value));
        }
    }
}
