using StudentManagement.WPF.ViewModels;
using System;
using System.Windows;

namespace StudentManagement.WPF;

public partial class MainWindow : Window
{
    public event Action? OnLogoutRequested;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        if (DataContext is ViewModels.MainWindowViewModel vm)
        {
            vm.OnLogoutRequested += () => OnLogoutRequested?.Invoke();
        }
    }
}
