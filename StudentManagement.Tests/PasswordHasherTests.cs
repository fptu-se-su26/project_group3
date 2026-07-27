using StudentManagement.Business.Security;
using Xunit;

namespace StudentManagement.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var hash = PasswordHasher.Hash("MySecret123");
            Assert.True(PasswordHasher.Verify("MySecret123", hash));
        }

        [Fact]
        public void Verify_WrongPassword_ReturnsFalse()
        {
            var hash = PasswordHasher.Hash("MySecret123");
            Assert.False(PasswordHasher.Verify("WrongPassword", hash));
        }

        [Fact]
        public void Hash_ProducesDifferentSaltEachTime()
        {
            var hash1 = PasswordHasher.Hash("SamePassword");
            var hash2 = PasswordHasher.Hash("SamePassword");
            Assert.NotEqual(hash1, hash2);
        }
    }
}
