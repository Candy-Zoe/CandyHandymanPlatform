using System.Windows;
using System.Windows.Controls;

namespace CandyHandyman.Desktop;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var dashboard = await App.ApiService.GetAsync<DashboardDto>("api/admin/dashboard");
            if (dashboard != null)
            {
                UserCount.Text = dashboard.totalUsers.ToString();
                OrderCount.Text = dashboard.totalOrders.ToString();
                ServiceCount.Text = dashboard.totalServices.ToString();
            }
        }
        catch
        {
            UserCount.Text = "0";
            OrderCount.Text = "0";
            ServiceCount.Text = "0";
        }
    }
}

public class DashboardDto
{
    public int totalUsers { get; set; }
    public int totalOrders { get; set; }
    public int totalServices { get; set; }
    public decimal totalRevenue { get; set; }
    public int pendingDisputes { get; set; }
}