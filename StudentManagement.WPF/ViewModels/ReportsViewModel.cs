using Microsoft.Win32;
using StudentManagement.Business.DTOs;
using StudentManagement.Business.Interfaces;
using StudentManagement.WPF.Commands;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly IFinanceGradeService _financeService;

        public ObservableCollection<string> ReportTypes { get; } = new ObservableCollection<string>
        {
            "Class Report", "Major Report", "Result Report", "Failure Report", "Unpaid Tuition Report", "Collected Tuition Report"
        };

        private string _selectedReportType = "Class Report";
        public string SelectedReportType { get => _selectedReportType; set { _selectedReportType = value; OnPropertyChanged(); } }

        private string _semesterFilter = string.Empty;
        public string SemesterFilter { get => _semesterFilter; set { _semesterFilter = value; OnPropertyChanged(); } }

        private IEnumerable _reportData = new ObservableCollection<object>();
        public IEnumerable ReportData { get => _reportData; set { _reportData = value; OnPropertyChanged(); } }

        public ICommand RunReportCommand { get; }
        public ICommand ExportCommand { get; }

        // Academic Results (F27)
        private string _resultStudentId = string.Empty;
        public string ResultStudentId { get => _resultStudentId; set { _resultStudentId = value; OnPropertyChanged(); } }

        private string _resultSemesterId = string.Empty;
        public string ResultSemesterId { get => _resultSemesterId; set { _resultSemesterId = value; OnPropertyChanged(); } }

        private AcademicResultSummary? _academicResult;
        public AcademicResultSummary? AcademicResult { get => _academicResult; set { _academicResult = value; OnPropertyChanged(); } }

        public ICommand ViewResultsCommand { get; }

        public ReportsViewModel(IFinanceGradeService financeService)
        {
            _financeService = financeService;
            RunReportCommand = new RelayCommand(async _ => await RunReportAsync());
            ExportCommand = new RelayCommand(_ => ExportReport());
            ViewResultsCommand = new RelayCommand(async _ => await ViewResultsAsync());
        }

        private async Task RunReportAsync()
        {
            try
            {
                ReportData = SelectedReportType switch
                {
                    "Class Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetClassReportAsync())),
                    "Major Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetMajorReportAsync())),
                    "Result Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetResultReportAsync(NullIfEmpty(SemesterFilter)))),
                    "Failure Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetFailureReportAsync(NullIfEmpty(SemesterFilter)))),
                    "Unpaid Tuition Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetUnpaidTuitionReportAsync())),
                    "Collected Tuition Report" => new ObservableCollection<object>(await ToObjectListAsync(_financeService.GetCollectedTuitionReportAsync())),
                    _ => new ObservableCollection<object>()
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running report: {ex.Message}");
            }
        }

        private static string? NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value;

        private static async Task<System.Collections.Generic.List<object>> ToObjectListAsync<T>(Task<System.Collections.Generic.IEnumerable<T>> source)
        {
            var result = await source;
            var list = new System.Collections.Generic.List<object>();
            foreach (var item in result) list.Add(item!);
            return list;
        }

        private void ExportReport()
        {
            var dlg = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", DefaultExt = ".json" };
            if (dlg.ShowDialog() == true)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(ReportData, options);
                System.IO.File.WriteAllText(dlg.FileName, json);
                MessageBox.Show("Report exported successfully.");
            }
        }

        private async Task ViewResultsAsync()
        {
            if (string.IsNullOrWhiteSpace(ResultStudentId) || string.IsNullOrWhiteSpace(ResultSemesterId))
            {
                MessageBox.Show("Please enter Student ID and Semester ID.");
                return;
            }

            try
            {
                AcademicResult = await _financeService.GetAcademicResultsAsync(ResultStudentId, ResultSemesterId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}");
            }
        }
    }
}
