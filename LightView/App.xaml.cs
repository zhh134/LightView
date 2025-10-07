using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading.Core;
using LightView.Windows;

namespace LightView
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _window;
        private AppInstance _currentInstance;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            UnhandledException += (s, e) =>
            {
                var window = new CrashPadWindow();
                window.ActivateWithCrashInfo(e.Exception.ToString());
                e.Handled = true;

            };
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                // Attempt to get activation args; may be null in some scenarios
                _currentInstance = AppInstance.GetCurrent();
                var activateArgs = _currentInstance?.GetActivatedEventArgs();

                // Ensure we have a MainWindow instance
                _window = MainWindow.Current ?? new MainWindow();
                _window.EnsureWindow();
                Frame rootFrame = _window.RootFrame;

                if (rootFrame is not null)
                {
                    // Safely handle file activation: check for nulls and use pattern matching
                    if (activateArgs != null && activateArgs.Kind == ExtendedActivationKind.File && activateArgs.Data is FileActivatedEventArgs fileArgs && fileArgs.Files != null && fileArgs.Files.Count > 0)
                    {
                        var list = fileArgs.Files.ToList();
                        rootFrame.Navigate(typeof(Pages.ImageViewerPage), list);
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(Pages.WelcomePage));
                    }
                }

                _window.Activate();
            }
            catch (Exception ex)
            {
                // Prevent app crash during activation; show crash info window and try to recover
                try
                {
                    var window = new CrashPadWindow();
                    window.ActivateWithCrashInfo(ex.ToString());
                }
                catch { }

                try
                {
                    // If window wasn't created successfully above, attempt to create and show a basic window
                    if (_window == null)
                    {
                        _window = MainWindow.Current ?? new MainWindow();
                        _window.EnsureWindow();
                        _window.Activate();
                    }
                }
                catch { }
            }
        }
    }
}
