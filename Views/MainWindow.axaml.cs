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
            string buttonText = button.Content.ToString();
            Debug.WriteLine($"Button clicked: {buttonText}");
            // Here you can add logic to switch the content based on the button clicked
        }
    }
}