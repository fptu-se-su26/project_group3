using System;
using System.Text.RegularExpressions;

namespace StudentManagement.Business.Validators
{
    public static class ValidationHelper
    {
        public const int MinPasswordLength = 6;

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static bool IsRequired(string? value) => !string.IsNullOrWhiteSpace(value);

        public static bool IsValidEmail(string? email) => IsRequired(email) && EmailRegex.IsMatch(email!);

        public static bool IsPastOrToday(DateTime date) => date.Date <= DateTime.Now.Date;

        public static bool IsInRange(double? value, double min, double max) => !value.HasValue || (value.Value >= min && value.Value <= max);

        public static bool IsPositive(int value) => value > 0;

        public static bool IsValidPassword(string? password) => IsRequired(password) && password!.Length >= MinPasswordLength;
    }
}
