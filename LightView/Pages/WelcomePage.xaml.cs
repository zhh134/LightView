using LightView.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LightView.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                //disable the button to avoid double-clicking
                button.IsEnabled = false;
                var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
                picker.CommitButtonText = "—°»°’’∆¨";
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.ViewMode = PickerViewMode.List;
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    MainWindow.Current.RootFrame.Navigate(typeof(ImageViewerPage), file.Path);
                }
                //re-enable the button
                button.IsEnabled = true;
            }

        }
    }
}
