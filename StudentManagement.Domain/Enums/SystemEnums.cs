namespace StudentManagement.Domain.Enums
{
    public enum StudentStatus { Studying, Reserved, Suspended, Graduated, DroppedOut }
    public enum ClassStatus { Active, Inactive }
    public enum MajorStatus { Active, Inactive }
    public enum LecturerStatus { Active, Inactive }
    public enum SubjectStatus { Active, Inactive }
    public enum CourseSectionStatus { Opened, Closed, Cancelled }
    public enum RegistrationStatus { Registered, Cancelled }
    public enum ResultClassification { Pass, Fail }
    public enum TuitionStatus { Unpaid, Partial, Paid }
    public enum PaymentMethod { Cash, BankTransfer }
}
