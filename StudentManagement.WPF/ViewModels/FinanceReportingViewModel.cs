using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;

namespace StudentManagement.WPF.ViewModels
{
    public class FinanceReportingViewModel : ViewModelBase
    {
        private readonly IFinanceGradeService _service;
        private readonly ICourseService _courseService;

        public ObservableCollection<Semester> Semesters { get; } = new();

        private Semester? _selectedSemester;
        public Semester? SelectedSemester
        {
            get => _selectedSemester;
            set { _selectedSemester = value; OnPropertyChanged(); _ = LoadDataAsync(); }
        }

        public decimal PricePerCredit { get; set; } = 1_000_000m;

        private ObservableCollection<Tuition> _tuitions = new ObservableCollection<Tuition>();
        public ObservableCollection<Tuition> Tuitions { get => _tuitions; set { _tuitions = value; OnPropertyChanged(); } }
        public Tuition? SelectedTuition { get; set; }

        public decimal PaymentAmount { get; set; }
        public PaymentMethod PaymentMethodSelected { get; set; } = PaymentMethod.Cash;
        public ObservableCollection<PaymentMethod> PaymentMethods { get; } = new(Enum.GetValues<PaymentMethod>());

        private decimal _totalRevenue;
        public decimal TotalRevenue { get => _totalRevenue; set { _totalRevenue = value; OnPropertyChanged(); } }

        private decimal _totalUnpaid;
        public decimal TotalUnpaid { get => _totalUnpaid; set { _totalUnpaid = value; OnPropertyChanged(); } }

        public ICommand LoadDataCommand { get; }
        public ICommand GenerateTuitionCommand { get; }
        public ICommand ProcessPaymentCommand { get; }

        public FinanceReportingViewModel(IFinanceGradeService service, ICourseService courseService)
        {
            _service = service;
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            GenerateTuitionCommand = new RelayCommand(async _ => await GenerateTuitionAsync());
            ProcessPaymentCommand = new RelayCommand(async _ => await ProcessPaymentAsync(), _ => SelectedTuition != null);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var semesters = await _courseService.GetAllSemestersAsync();
            foreach (var s in semesters) Semesters.Add(s);
            SelectedSemester = Semesters.Count > 0 ? Semesters[0] : null;
        }

        private async Task LoadDataAsync()
        {
            if (SelectedSemester == null) return;
            try
            {
                var tuitionsList = await _service.GetAllTuitionsAsync(SelectedSemester.SemesterId);
                Tuitions = new ObservableCollection<Tuition>(tuitionsList);

                TotalRevenue = Tuitions.Sum(t => t.PaidAmount);
                TotalUnpaid = Tuitions.Sum(t => t.Amount - t.PaidAmount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading finance data: {ex.Message}");
            }
        }

        private async Task GenerateTuitionAsync()
        {
            if (SelectedSemester == null) return;
            try
            {
                await _service.GenerateTuitionForSemesterAsync(SelectedSemester.SemesterId, PricePerCredit);
                MessageBox.Show("Tuition generated successfully.");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating tuition: {ex.Message}");
            }
        }

        private async Task ProcessPaymentAsync()
        {
            if (SelectedTuition == null) return;

            var (isSuccess, message) = await _service.ProcessPaymentAsync(SelectedTuition.TuitionId, PaymentAmount, PaymentMethodSelected, "Payment recorded via Finance screen");
            MessageBox.Show(message);

            if (isSuccess)
            {
                PaymentAmount = 0;
                await LoadDataAsync();
            }
        }
    }
}
