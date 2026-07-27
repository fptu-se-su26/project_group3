using Microsoft.EntityFrameworkCore;
using StudentManagement.DataAccess;
using System;

namespace StudentManagement.Tests
{
    public static class TestDbFactory
    {
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
    }
}
