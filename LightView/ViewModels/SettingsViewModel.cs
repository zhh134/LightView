using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LightView.Windows;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;

namespace LightView.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const string ThemeSettingKey = "AppTheme";

        public SettingsViewModel()
        {
            Initialize();
        }

        [ObservableProperty]
        private AppTheme _selectedTheme;

        [ObservableProperty]
        private AppInfo _appInformation;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<AppTheme> AvailableThemes { get; } = new()
    {
        AppTheme.Light,
        AppTheme.Dark,
        AppTheme.System
    };

        private void Initialize()
        {
            // 从本地设置加载当前主题
            LoadCurrentTheme();

            // 加载应用信息
            LoadAppInformation();

            // 注册属性变更事件
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedTheme))
            {
                OnThemeChanged();
            }
        }

        private void LoadCurrentTheme()
        {
            if (ApplicationData.GetDefault().LocalSettings.Values.TryGetValue(ThemeSettingKey, out var savedTheme))
            {
                SelectedTheme = (AppTheme)savedTheme;
            }
            else
            {
                SelectedTheme = AppTheme.System; // 默认值
            }
        }

        private async void OnThemeChanged()
        {
            // 保存主题设置
            ApplicationData.GetDefault().LocalSettings.Values[ThemeSettingKey] = (int)SelectedTheme;

            // 应用主题变更
            await ApplyThemeAsync(SelectedTheme);
        }

        [RelayCommand]
        private async Task LoadAppInformation()
        {
            IsLoading = true;

            
        }

        [RelayCommand]
        private async Task RefreshAppInfo()
        {
            await LoadAppInformation();
        }

        [RelayCommand]
        private void ResetToDefaultTheme()
        {
            SelectedTheme = AppTheme.System;
        }


        [RelayCommand]
        private async Task OpenAppWebsite()
        {
            try
            {
                await Launcher.LaunchUriAsync(
                    new Uri("https://yourapp.com"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"打开应用网站失败: {ex.Message}");
            }
        }

        private string GetCopyrightInfo()
        {
            try
            {
                var package = Package.Current;
                if (package != null)
                {
                    // 尝试从包信息获取版权信息
                    // 在实际应用中，你可能需要从其他来源获取
                    return $"© {DateTime.Now.Year} {package.PublisherDisplayName}";
                }
            }
            catch
            {
                // 忽略错误
            }

            return $"© {DateTime.Now.Year} Your Company";
        }

        private async Task ApplyThemeAsync(AppTheme theme)
        {
            ApplyThemeToWindow(theme);
        }

        private void ApplyThemeToWindow(AppTheme theme)
        {
            // 获取当前窗口并应用主题
            var window = MainWindow.Current;
            if (window?.Content is Microsoft.UI.Xaml.FrameworkElement rootElement)
            {
                switch (theme)
                {
                    case AppTheme.Light:
                        rootElement.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
                        break;
                    case AppTheme.Dark:
                        rootElement.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark;
                        break;
                    case AppTheme.System:
                        rootElement.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Default;
                        break;
                }
            }

            
        }

        // 清理资源
        public void Dispose()
        {
            PropertyChanged -= OnPropertyChanged;
        }
    }

    public enum AppTheme
    {
        Light,
        Dark,
        System

    }
}
