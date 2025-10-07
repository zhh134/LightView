using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;

namespace LightView.Utils
{
    public static class PrintUIHelper
    {
        public static PrintManager GetPrintManagerForWindow(this Window window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return PrintManagerInterop.GetForWindow(hWnd);
        }

        public static async Task ShowPrintUIForWindow(this Window window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            await PrintManagerInterop.ShowPrintUIForWindowAsync(hWnd);
        }
    }
}
