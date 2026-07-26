using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class GradeManagementViewModel : ViewModelBase
    {
        private readonly IFinanceGradeService _service;
        private readonly string _currentSectionId = "CS101-SP24"; // Hardcoded for mockup

        private ObservableCollection<Grade> _grades = new ObservableCollection<Grade>();
        public ObservableCollection<Grade> Grades { get => _grades; set { _grades = value; OnPropertyChanged(); } }
        public Grade? SelectedGrade { get; set; }

        public ICommand LoadDataCommand { get; }
        public ICommand SaveGradeCommand { get; }

        public GradeManagementViewModel(IFinanceGradeService service)
        {
            _service = service;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            SaveGradeCommand = new RelayCommand(async _ => await SaveGradeAsync(), _ => SelectedGrade != null);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var gradesList = await _service.GetGradesForSectionAsync(_currentSectionId);
                Grades = new ObservableCollection<Grade>(gradesList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grades: {ex.Message}");
            }
        }

        private async Task SaveGradeAsync()
        {
            if (SelectedGrade == null) return;
            try
            {
                await _service.UpdateGradeAsync(SelectedGrade.GradeId, SelectedGrade.Assignment, SelectedGrade.ProgressTest, SelectedGrade.Practical, SelectedGrade.FinalExam);
                MessageBox.Show("Grade saved successfully.");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving grade: {ex.Message}");
            }
        }
    }
}
