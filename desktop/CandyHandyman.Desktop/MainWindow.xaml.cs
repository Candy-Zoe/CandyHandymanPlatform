using System.Windows;
using System.Windows.Controls;

namespace CandyHandyman.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new DashboardView();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            MainContent.Content = tag switch
            {
                "Dashboard" => new DashboardView(),
                "Users" => new UserManagementView(),
                "Services" => new ServiceManagementView(),
                "Orders" => new OrderManagementView(),
                "Categories" => new CategoryManagementView(),
                "Reviews" => new ReviewManagementView(),
                "Verification" => new VerificationManagementView(),
                "Certifications" => new CertificationManagementView(),
                "Disputes" => new DisputeManagementView(),
                "Insurance" => new InsuranceManagementView(),
                _ => new DashboardView()
            };
        }
    }
}