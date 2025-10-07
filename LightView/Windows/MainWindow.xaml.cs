using LightView.Pages;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;


namespace LightView.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public new static MainWindow Current => _current ??= new MainWindow();
        private static MainWindow _current;

        public static List<MainWindow> Windows => _windows ??= new List<MainWindow>();
        private static List<MainWindow> _windows;

        public Frame RootFrame => rootFrame;
        public Frame SettingsFrame => settingsFrame;

        public MainWindow()
        {
            InitializeComponent();
            Windows.Add(this);

            Closed += (s, e) => Windows.Remove(this);
        }

        public void EnsureWindow()
        {
            var appWindowTitleBar = AppWindow.TitleBar;
            appWindowTitleBar.ExtendsContentIntoTitleBar = true;
            appWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonHoverBackgroundColor = (App.Current as App).RequestedTheme == 
                ApplicationTheme.Dark ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(30, 0, 0, 0);
            appWindowTitleBar.ButtonPressedBackgroundColor = (App.Current as App).RequestedTheme ==
                ApplicationTheme.Dark ? Color.FromArgb(60, 255, 255, 255) : Color.FromArgb(60, 0, 0, 0);
            appWindowTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appWindowTitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        }

        public void ShowSettings()
        {
            if (SettingsLayer.Visibility == Visibility.Visible)
            {
                SettingsLayer.Visibility = Visibility.Collapsed;
                SettingsFrame.Content = null;
                return;
            }
            SettingsLayer.Visibility = Visibility.Visible;
            SettingsFrame.Navigate(typeof(SettingsPage), null, new DrillInNavigationTransitionInfo());
        }

        private void SettingsTitleBar_BackRequested(TitleBar sender, object args)
        {
            ShowSettings();
        }
    }
}
