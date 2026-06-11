using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ControllerWrangler.ViewModels;
using ControllerWrangler.Views;
using System.Diagnostics;
using ControllerWrangler.Driver;

namespace ControllerWrangler;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Debug.WriteLine("Welcome to Controller Wrangler!");

        //TESTING ON BOOT - REMOVE LATER
        //var driverController = new RadVRController();
        //driverController.Connect();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}