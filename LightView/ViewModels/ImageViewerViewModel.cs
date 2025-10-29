using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightView.Models;
using LightView.Utils;
using LightView.Windows;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.WebUI;

namespace LightView.ViewModels
{
    public partial class ImageViewerViewModel : ObservableRecipient
    {
        [ObservableProperty] private ObservableCollection<ImageModel> _images;
        [ObservableProperty] private int _currentIndex;
        [ObservableProperty] private ImageModel _currentImage;
        [ObservableProperty] private ImageResolutionInfo _currentImageResolutionInfo;
        [ObservableProperty] private PrintDocument _printDocument;
        [ObservableProperty] private IPrintDocumentSource _printDocumentSource;
        [ObservableProperty] private string _displayInfo;
        [ObservableProperty] private ObservableCollection<ImageModel> _folderImages = new ObservableCollection<ImageModel>();

        private readonly DataTransferManager dataTransferManager
            = MainWindow.Current.GetForWindow();
        private readonly PrintManager printManager
            = MainWindow.Current.GetPrintManagerForWindow();
        private List<UIElement> printPreviewPages = new List<UIElement>();

        public ImageViewerViewModel()
        {
            Images = new ObservableCollection<ImageModel>();
            FolderImages = new ObservableCollection<ImageModel>();
        }

        partial void OnCurrentImageChanged(ImageModel value)
        {
            CurrentImageResolutionInfo = ImageResolutionHelper.GetDetailedResolutionInfo(CurrentImage.PathOringinalString);
            DisplayInfo = $"{CurrentImageResolutionInfo.Width}x{CurrentImageResolutionInfo.Height} - {CurrentImage.Type}";
        }

        public void RegisterForPrinting()
        {
            printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;

            PrintDocument = new PrintDocument();
            PrintDocumentSource = PrintDocument.DocumentSource;
            PrintDocument.Paginate += PrintDocument_Paginate;
            PrintDocument.GetPreviewPage += PrintDocument_GetPreviewPage;
            PrintDocument.AddPages += PrintDocument_AddPages;
        }

        public void UnregisterForPrinting()
        {
            printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;

            PrintDocument = new PrintDocument();
            PrintDocumentSource = PrintDocument.DocumentSource;
            PrintDocument.Paginate += PrintDocument_Paginate;
            PrintDocument.GetPreviewPage += PrintDocument_GetPreviewPage;
            PrintDocument.AddPages += PrintDocument_AddPages;
        }

        private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            PrintDocument printDocument = (PrintDocument)sender;

            // Loop over all of the preview pages and add each one to be printed.
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                printDocument.AddPage(printPreviewPages[i]);
            }

            // Indicate that all of the print pages have been provided.
            printDocument.AddPagesComplete();
        }

        private void PrintDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDocument = (PrintDocument)sender;
            printDocument.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        {
            printPreviewPages.Clear();

            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            Image printImage = new Image();
            printImage.Source = CurrentImage.ImageSource;

            printPreviewPages.Add(printImage);

            PrintDocument printDocument = (PrintDocument)sender;
            printDocument.SetPreviewPageCount(printPreviewPages.Count,
                                                  PreviewPageCountType.Intermediate);
        }

        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask printTask = args.Request.CreatePrintTask("LightView 照片查看器", PrintTaskSourceRequested);

            printTask.Completed += PrintTask_Completed;
        }

        private void PrintTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
        {

        }

        private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(PrintDocumentSource);
        }

        // Make the event handler async so we can await StorageFile APIs instead of blocking with .Result
        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            try
            {
                if (CurrentImage != null)
                {
                    request.Data.Properties.Title = "分享照片";
                    request.Data.Properties.Description = "分享来自 LightView 的图片";

                    // Use await instead of .Result to avoid blocking the UI thread
                    var storageFile = await StorageFile.GetFileFromPathAsync(CurrentImage.PathOringinalString);
                    request.Data.SetStorageItems(new[] { storageFile });
                }
                else
                {
                    request.FailWithDisplayText("No image to share.");
                }
            }
            catch (Exception ex)
            {
                // If file access or other errors occur, fail the request with a friendly message
                request.FailWithDisplayText($"分享失败: {ex.Message}");
            }
            finally
            {
                // Unsubscribe to avoid multiple registrations / memory leaks
                try
                {
                    if (dataTransferManager != null)
                    {
                        dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        [RelayCommand]
        private void OnShareImage()
        {
            if (CurrentImage == null)
            {
                DisplayInfo = "没有可分享的图片。";
                return;
            }

            try
            {
                // Get the DataTransferManager for the current view and register the handler
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;

                // This call can throw COMException when the app/window is not in the foreground.
                // Catch the exception and set a friendly message so the UI can inform the user.
                MainWindow.Current.ShowShareUI();
            }
            catch (Exception ex)
            {
                // Generic fallback
                DisplayInfo = $"分享失败: {ex.Message}";
                Task.Delay(3000).ContinueWith(_ => DisplayInfo = $"{CurrentImageResolutionInfo.Width}x{CurrentImageResolutionInfo.Height} - {CurrentImage.Type}");
                try
                {
                    if (dataTransferManager != null)
                    {
                        dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
                    }
                }
                catch { }
            }
        }

        [RelayCommand]
        private async Task OnPrintImage()
        {
            if (CurrentImage == null)
            {
                DisplayInfo = "没有可分享的图片。";
                return;
            }


            if (PrintManager.IsSupported())
            {
                try
                {
                    await MainWindow.Current.ShowPrintUIForWindow();
                }
                catch
                {

                }
            }

        }

        public async Task LoadImagesFromFolderAsync(string imagePath)
        {
            try
            {
                var folderPath = Path.GetDirectoryName(imagePath);
                if (folderPath == null) return;

                var files = Directory.EnumerateFiles(folderPath)
                    .Where(file => IsImageFile(Path.GetExtension(file)))
                    .ToList();

                Debug.WriteLine($"Loading {files.Count} images from folder: {folderPath}");

                FolderImages.Clear();
                foreach (var file in files)
                {
                    Debug.WriteLine($"Adding image: {file}");
                    FolderImages.Add(ImageModel.CreateFromPath(file));
                }

                Debug.WriteLine($"FolderImages now contains {FolderImages.Count} items.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading images: {ex.Message}");
            }
        }

        public void OpenImage(string path)
        {
            CurrentImage = ImageModel.CreateFromPath(path) ?? new ImageModel();

            if (CurrentImage != null)
            {
                LoadImagesFromFolderAsync(CurrentImage.PathOringinalString).SafeFireAndForget();
            }
        }

        private bool IsImageFile(string fileType)
        {
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp", ".heic" };
            return supportedExtensions.Contains(fileType.ToLower());
        }
    }

}