using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace LightView.Utils
{
    public static class DataTransferHelper
    {
        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
        [System.Runtime.InteropServices.InterfaceType(
        System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        interface IDataTransferManagerInterop
        {
            IntPtr GetForWindow([System.Runtime.InteropServices.In] IntPtr appWindow,
                [System.Runtime.InteropServices.In] ref Guid riid);
            void ShowShareUIForWindow(IntPtr appWindow);
        }

        static readonly Guid _dtm_iid =
            new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

        public static DataTransferManager GetForWindow(this Window window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            IDataTransferManagerInterop interop =
            DataTransferManager.As
                <IDataTransferManagerInterop>();

            IntPtr result = interop.GetForWindow(hWnd, _dtm_iid);
            return WinRT.MarshalInterface<DataTransferManager>.FromAbi(result);
        }

        public static void ShowShareUI(this Window window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            IDataTransferManagerInterop interop =
            DataTransferManager.As<IDataTransferManagerInterop>();

            interop.ShowShareUIForWindow(hWnd);
        }
    }
}
