using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;


namespace LightView.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CrashPadWindow : WindowEx
{
    private string _crashInfo;

    public CrashPadWindow()
    {
        InitializeComponent();
        _crashInfo = "";
        EnsureWindow();
    }

    private void EnsureWindow()
    {
        var appWindowTitleBar = AppWindow.TitleBar;
        appWindowTitleBar.ExtendsContentIntoTitleBar = true;
        appWindowTitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
        appWindowTitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        appWindowTitleBar.ButtonHoverBackgroundColor = (App.Current as App).RequestedTheme ==
            ApplicationTheme.Dark ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(30, 0, 0, 0);
        appWindowTitleBar.ButtonPressedBackgroundColor = (App.Current as App).RequestedTheme ==
            ApplicationTheme.Dark ? Color.FromArgb(60, 255, 255, 255) : Color.FromArgb(60, 0, 0, 0);
        appWindowTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        appWindowTitleBar.PreferredTheme = (App.Current as App).RequestedTheme ==
            ApplicationTheme.Dark ? TitleBarTheme.Dark : TitleBarTheme.Dark;

        var presenter = OverlappedPresenter.CreateForDialog();
        AppWindow.SetPresenter(presenter);

        Height = 400;
        Width = 600;
        IsMaximizable = false;
        IsMinimizable = false;
        
    }

    public void ActivateWithCrashInfo(string crashInfo)
    {
        _crashInfo = crashInfo;
        if (!AppWindow.IsVisible)
        {
            Activate();
        }
    }
}
