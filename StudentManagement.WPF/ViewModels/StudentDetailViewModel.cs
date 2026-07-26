using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class StudentDetailViewModel : ViewModelBase
    {
        private readonly IStudentService _studentService;
        private readonly bool _isEditMode;

        public Student CurrentStudent { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public delegate void CloseActionHandler();
        public event CloseActionHandler? OnRequestClose;

        public StudentDetailViewModel(IStudentService studentService, Student? student = null)
        {
            _studentService = studentService;
            if (student == null)
            {
                CurrentStudent = new Student { EnrollmentDate = DateTime.Now, DateOfBirth = DateTime.Now };
                _isEditMode = false;
            }
            else
            {
                CurrentStudent = student; // Need deep copy in reality, using ref for simplicity
                _isEditMode = true;
            }

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => OnRequestClose?.Invoke());
        }

        private async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentStudent.StudentId) || string.IsNullOrWhiteSpace(CurrentStudent.FullName))
                {
                    MessageBox.Show("ID and Name are required.", "Validation Error");
                    return;
                }

                if (_isEditMode)
                {
                    await _studentService.UpdateStudentAsync(CurrentStudent);
                }
                else
                {
                    await _studentService.AddStudentAsync(CurrentStudent);
                }
                OnRequestClose?.Invoke();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving");
            }
        }
    }
}
