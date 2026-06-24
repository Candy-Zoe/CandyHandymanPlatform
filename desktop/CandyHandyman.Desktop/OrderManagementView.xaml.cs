using System.Windows;
using System.Windows.Controls;

namespace CandyHandyman.Desktop;

public partial class OrderManagementView : UserControl
{
    public OrderManagementView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadOrders();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadOrders();
    }

    private async System.Threading.Tasks.Task LoadOrders()
    {
        try
        {
            var orders = await App.ApiService.GetAsync<List<OrderDto>>("api/admin/orders");
            if (orders != null)
            {
                OrdersGrid.ItemsSource = orders;
            }
        }
        catch { }
    }
}

public class OrderDto
{
    public string Id { get; set; } = "";
    public string OrderNo { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string Address { get; set; } = "";
    public string CreatedAt { get; set; } = "";
}