using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
//using ControllerWrangler.Views;

namespace ControllerWrangler.Views;

public partial class MainWindow : Window
{
    //store Contentview
    //private readonly UserControl _mainContentView = new MainContentView();
    //private readonly UserControl _settingContentView = new UserControl();

    public MainWindow()
    {
        InitializeComponent();
        // Set initial content to MainContentView6
        //MainSplitView.Content = _mainContentView;
    }

    private void Pannel_button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string? param = button.CommandParameter?.ToString();
            Debug.WriteLine($"Button clicked: {param}");
            // Here you can add logic to switch the content based on the button clicked
            switch (param)
            {
                case "Main":
                    //MainSplitView.Content = _mainContentView;
                    Debug.WriteLine("Switched to MainContentView");
                    break;
                case "Settings":
                    //MainSplitView.Content = _settingContentView;
                    Debug.WriteLine("Switched to SettingContentView");
                    break;
                default:
                    Debug.WriteLine($"No match for param: '{param}'");
                    break;
            }
        }
    }
}