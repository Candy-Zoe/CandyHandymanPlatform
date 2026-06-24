using System.Windows;
using System.Windows.Controls;

namespace CandyHandyman.Desktop;

public partial class UserManagementView : UserControl
{
    public UserManagementView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadUsers();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadUsers();
    }

    private async System.Threading.Tasks.Task LoadUsers()
    {
        try
        {
            var users = await App.ApiService.GetAsync<List<UserDto>>("api/admin/users");
            if (users != null)
            {
                UsersGrid.ItemsSource = users;
            }
        }
        catch { }
    }
}

public class UserDto
{
    public string Id { get; set; } = "";
    public string Nickname { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public decimal Balance { get; set; }
    public string CreatedAt { get; set; } = "";
}