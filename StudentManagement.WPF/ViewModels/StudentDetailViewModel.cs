using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Enums;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class StudentDetailViewModel : ViewModelBase
    {
        private readonly IStudentService _studentService;
        private readonly bool _isEditMode;
        private readonly StudentStatus _originalStatus;

        public Student CurrentStudent { get; set; }

        public ObservableCollection<StudentStatus> StatusOptions { get; } = new(Enum.GetValues<StudentStatus>());

        private string? _statusReason;
        public string? StatusReason { get => _statusReason; set { _statusReason = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public delegate void CloseActionHandler();
        public event CloseActionHandler? OnRequestClose;

        public StudentDetailViewModel(IStudentService studentService, Student? student = null)
        {
            _studentService = studentService;
            if (student == null)
            {
                CurrentStudent = new Student { EnrollmentDate = DateTime.Now, DateOfBirth = DateTime.Now, Status = StudentStatus.Studying };
                _isEditMode = false;
            }
            else
            {
                CurrentStudent = student;
                _isEditMode = true;
            }
            _originalStatus = CurrentStudent.Status;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => OnRequestClose?.Invoke());
        }

        private async Task SaveAsync()
        {
            try
            {
                (bool IsSuccess, string Message) result;
                var statusChanged = _isEditMode && CurrentStudent.Status != _originalStatus;

                // Validate the status-change reason before persisting anything, so a
                // rejected reason never leaves the profile fields saved with a
                // half-applied status change.
                if (statusChanged
                    && (CurrentStudent.Status == StudentStatus.Suspended || CurrentStudent.Status == StudentStatus.DroppedOut)
                    && string.IsNullOrWhiteSpace(StatusReason))
                {
                    MessageBox.Show("A reason is required for this status change.", "Validation Error");
                    return;
                }

                if (_isEditMode)
                {
                    result = await _studentService.UpdateStudentAsync(CurrentStudent);
                    if (result.IsSuccess && statusChanged)
                    {
                        result = await _studentService.UpdateStudentStatusAsync(CurrentStudent.StudentId, CurrentStudent.Status, StatusReason);
                    }
                }
                else
                {
                    result = await _studentService.AddStudentAsync(CurrentStudent);
                }

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Validation Error");
                    return;
                }

                OnRequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving");
            }
        }
    }
}
