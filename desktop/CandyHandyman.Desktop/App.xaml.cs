using System.Windows;

namespace CandyHandyman.Desktop;

public partial class App : Application
{
    public static Services.ApiService ApiService { get; } = new();
}