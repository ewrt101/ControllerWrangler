using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace ControllerWrangler.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Pannel_button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string param = button.CommandParameter?.ToString();
            Debug.WriteLine($"Button clicked: {param}");
            // Here you can add logic to switch the content based on the button clicked
            switch (param)
            {
                case "Main":
                    MainSplitView.Content = new MainContentView();
                    Debug.WriteLine("Switched to MainContentView");
                    break;
                case "Settings":
                    MainSplitView.Content = new SettingContentView();
                    Debug.WriteLine("Switched to SettingContentView");
                    break;
                default:
                    Debug.WriteLine($"No match for param: '{param}'");
                    break;
            }
        }
    }
}