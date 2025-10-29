using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using LightView.Utils;
using LightView.Models;
using LightView.ViewModels;
using LightView.Windows;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using System;
using AsyncAwaitBestPractices;


namespace LightView.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageViewerPage : Page
    {
        private ImageViewerViewModel ViewModel = new ImageViewerViewModel();

        public ImageViewerPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.RegisterForPrinting();

            try
            {
                if (e.Parameter is List<IStorageItem> storageItems)
                {
                    storageItems.Select(t => t.MapToImageModel()).ToList()
                        .ForEach(t => ViewModel.Images.Add(t));
                    ViewModel.CurrentImage = ViewModel.Images.FirstOrDefault() ?? new ImageModel();

                    if (ViewModel.CurrentImage != null)
                    {
                        ViewModel.LoadImagesFromFolderAsync(ViewModel.CurrentImage.PathOringinalString).SafeFireAndForget();
                    }
                }
                else if (e.Parameter is string path)
                {
                    ViewModel.OpenImage(path);
                }
                else
                {
                    MainWindow.Current.RootFrame.Navigate(typeof(WelcomePage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during navigation to ImageViewerPage: {ex.Message}");
                MainWindow.Current.RootFrame.Navigate(typeof(WelcomePage));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.UnregisterForPrinting();
        }

        private void TitleBar_BackRequested(TitleBar sender, object args)
        {
            MainWindow.Current.RootFrame.Navigate(typeof(WelcomePage));
        }

        private void SettingsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainWindow.Current.ShowSettings();
        }

       
        private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
        {
            var img = args.InvokedItem as ImageModel;
            ViewModel.OpenImage(img?.PathOringinalString);
        }
    }
}
