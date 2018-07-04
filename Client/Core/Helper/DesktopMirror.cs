using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace xClient.Core.Helper
{
    public class DesktopMirror : IDisposable
    {
        #region External Constants

        private const int Map = 1030;
        private const int UnMap = 1031;
        private const int TestMapped = 1051;
        private const int CDS_UPDATEREGISTRY = 0x00000001;
        private const int DM_BITSPERPEL = 0x40000;
        private const int DM_PELSWIDTH = 0x80000;
        private const int DM_PELSHEIGHT = 0x100000;
        private const int DM_POSITION = 0x00000020;

        #endregion

        #region External Methods

        [DllImport("user32.dll")]
        private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DeviceMode mode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr pointer);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayDevices(string lpDevice, uint ideviceIndex, ref DisplayDevice lpdevice, uint dwFlags);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern int ExtEscape(IntPtr hdc, int nEscape, int cbInput, IntPtr lpszInData, int cbOutput, IntPtr lpszOutData);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        private static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #endregion

        #region External Struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DisplayDevice
        {
            public int CallBack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DeviceMode
        {

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmSize;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmFields;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmOrientation;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmPaperSize;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmPaperLength;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmPaperWidth;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmScale;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmCopies;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmDefaultSource;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmPrintQuality;


            [MarshalAs(UnmanagedType.I2)] // short
            public short dmColor;

            [MarshalAs(UnmanagedType.I2)] // short
            public short dmDuplex;

            [MarshalAs(UnmanagedType.I2)] // short
            public short dmYResolution;

            [MarshalAs(UnmanagedType.I2)] // short
            public short dmTTOption;

            [MarshalAs(UnmanagedType.I2)] // short
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            [MarshalAs(UnmanagedType.U2)] // WORD
            public short dmLogPixels;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmICMMethod;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmICMIntent;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmMediaType;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmDitherType;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmReserved1;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmReserved2;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)] // DWORD
            public int dmPanningHeight;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct GetChangesBuffer
        {
            /// <summary>
            /// Pointer to the <see cref="ChangesBuffer"/> structure. To be casted to the neccesary OperationType a bit later...
            /// </summary>
            public IntPtr Buffer;

            public IntPtr UserBuffer;
        }
        #endregion

        private const string driverDeviceNumber = "DEVICE0";
        private const string driverMiniportName = "dfmirage";
        private const string driverName = "Mirage Driver";
        private const string driverRegistryPath = "SYSTEM\\CurrentControlSet\\Hardware Profiles\\Current\\System\\CurrentControlSet\\Services";

        private RegistryKey _registryKey;
        private IntPtr _globalDC;
        private string driverInstanceName = "";
        private IntPtr _getChangesBuffer = IntPtr.Zero;
        private int _bitmapWidth, _bitmapHeight, _bitmapBpp, _bitmapStride;

        private static void SafeChangeDisplaySettingsEx(string lpszDeviceName, ref DeviceMode mode, IntPtr hwnd, uint dwflags, IntPtr lParam)
        {
            int result = ChangeDisplaySettingsEx(lpszDeviceName, ref mode, hwnd, dwflags, lParam);
            switch (result)
            {
                case 0:
                    return; //DISP_CHANGE_SUCCESSFUL
                case 1:
                    throw new Exception("The computer must be restarted for the graphics mode to work."); //DISP_CHANGE_RESTART
                case -1:
                    throw new Exception("The display driver failed the specified graphics mode."); // DISP_CHANGE_FAILED
                case -2:
                    throw new Exception("The graphics mode is not supported."); // DISP_CHANGE_BADMODE
                case -3:
                    throw new Exception("Unable to write settings to the registry."); // DISP_CHANGE_NOTUPDATED
                case -4:
                    throw new Exception("An invalid set of flags was passed in."); // DISP_CHANGE_BADFLAGS
                case -5:
                    throw new Exception("An invalid parameter was passed in. This can include an invalid flag or combination of flags.");
                // DISP_CHANGE_BADPARAM
                case -6:
                    throw new Exception("The settings change was unsuccessful because the system is DualView capable."); // DISP_CHANGE_BADDUALVIEW
            }
        }

        public enum MirrorState
        {
            Idle,
            Loaded,
            Connected,
            Running
        }

        public MirrorState State { get; private set; }
        public bool Connected
        {
            get
            {
                return State == MirrorState.Connected;
            }
        }
        public bool DriverExists()
        {
            var device = new DisplayDevice();
            var deviceMode = new DeviceMode { dmDriverExtra = 0 };

            device.CallBack = Marshal.SizeOf(device);
            deviceMode.dmSize = (short)Marshal.SizeOf(deviceMode);
            deviceMode.dmBitsPerPel = Screen.PrimaryScreen.BitsPerPixel;

            if (deviceMode.dmBitsPerPel == 24)
                deviceMode.dmBitsPerPel = 32;

            _bitmapBpp = deviceMode.dmBitsPerPel;

            deviceMode.dmDeviceName = string.Empty;
            deviceMode.dmFields = (DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_POSITION);
            _bitmapHeight = deviceMode.dmPelsHeight = Screen.PrimaryScreen.Bounds.Height;
            _bitmapWidth = deviceMode.dmPelsWidth = Screen.PrimaryScreen.Bounds.Width;

            bool deviceFound;
            uint deviceIndex = 0;

            while (deviceFound = EnumDisplayDevices(null, deviceIndex, ref device, 0))
            {
                if (device.DeviceString == driverName)
                    break;
                deviceIndex++;
            }

            if (!deviceFound)
                return false;
            else
                return true;
        }
        public bool Load()
        {
            if (State != MirrorState.Idle)
                throw new InvalidOperationException("You may call Load only if the state is Idle");

            var device = new DisplayDevice();
            var deviceMode = new DeviceMode { dmDriverExtra = 0 };

            device.CallBack = Marshal.SizeOf(device);
            deviceMode.dmSize = (short)Marshal.SizeOf(deviceMode);
            deviceMode.dmBitsPerPel = Screen.PrimaryScreen.BitsPerPixel;

            if (deviceMode.dmBitsPerPel == 24)
                deviceMode.dmBitsPerPel = 32;

            _bitmapBpp = deviceMode.dmBitsPerPel;
            deviceMode.dmDeviceName = string.Empty;
            deviceMode.dmFields = (DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_POSITION);
            _bitmapHeight = deviceMode.dmPelsHeight = Screen.PrimaryScreen.Bounds.Height;
            _bitmapWidth = deviceMode.dmPelsWidth = Screen.PrimaryScreen.Bounds.Width;
            _bitmapStride = _bitmapWidth * _bitmapBpp/8;
            var d = _bitmapStride % 4;
            if (d != 0)
            {
                _bitmapStride += (4 - d);
            }
            bool deviceFound;
            uint deviceIndex = 0;

            while (deviceFound = EnumDisplayDevices(null, deviceIndex, ref device, 0))
            {
                if (device.DeviceString == driverName)
                    break;
                deviceIndex++;
            }
            if (!deviceFound)
                return false;
            driverInstanceName = device.DeviceName;
            _registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(driverRegistryPath, true);
            if (_registryKey != null)
                _registryKey = _registryKey.CreateSubKey(driverMiniportName);
            else
                throw new Exception("Couldn't open registry key");

            if (_registryKey != null)
                _registryKey = _registryKey.CreateSubKey(driverDeviceNumber);
            else
                throw new Exception("Couldn't open registry key");

            _registryKey.SetValue("Screen.ForcedBpp", 32);
            _registryKey.SetValue("Attach.ToDesktop", 1);

            #region This was CommitDisplayChanges

            SafeChangeDisplaySettingsEx(device.DeviceName, ref deviceMode, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
            SafeChangeDisplaySettingsEx(device.DeviceName, ref deviceMode, IntPtr.Zero, 0, IntPtr.Zero);

            #endregion

            State = MirrorState.Loaded;
            bool result = MapSharedBuffers();
            if (result)
            {
                State = MirrorState.Connected;
            }
            return result;
        }
        public bool Connect()
        {
            if (State != MirrorState.Loaded)
                throw new InvalidOperationException("You may call Connect only if the state is Loaded");

            bool result = MapSharedBuffers(); 
            if (result)
            {
                State = MirrorState.Connected;
            }
            return result;
        }
        public void Disconnect()
        {

            if (State != MirrorState.Connected)
                return;
            UnmapSharedBuffers();
            State = MirrorState.Loaded;
        }
        public void Unload()
        {
            if (State == MirrorState.Connected)
                Disconnect();
            if (State != MirrorState.Loaded)
                return;
            var deviceMode = new DeviceMode();
            deviceMode.dmSize = (short)Marshal.SizeOf(typeof(DeviceMode));
            deviceMode.dmDriverExtra = 0;
            deviceMode.dmFields = (DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_POSITION);

            var device = new DisplayDevice();
            device.CallBack = Marshal.SizeOf(device);
            deviceMode.dmDeviceName = string.Empty;
            uint deviceIndex = 0;
            while (EnumDisplayDevices(null, deviceIndex, ref device, 0))
            {
                if (device.DeviceString.Equals(driverName))
                    break;

                deviceIndex++;
            }

            Debug.Assert(_registryKey != null);

            _registryKey.SetValue("Attach.ToDesktop", 0);
            _registryKey.Close();

            deviceMode.dmDeviceName = driverMiniportName;

            if (deviceMode.dmBitsPerPel == 24) deviceMode.dmBitsPerPel = 32;

            #region This was CommitDisplayChanges

            SafeChangeDisplaySettingsEx(device.DeviceName, ref deviceMode, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
            SafeChangeDisplaySettingsEx(device.DeviceName, ref deviceMode, IntPtr.Zero, 0, IntPtr.Zero);

            #endregion

            State = MirrorState.Idle;
        }
        private bool MapSharedBuffers()
        {
            _globalDC = CreateDC(driverInstanceName, null, null, IntPtr.Zero);
            if (_globalDC == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (_getChangesBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_getChangesBuffer);
            }

            _getChangesBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GetChangesBuffer)));

            int res = ExtEscape(_globalDC, Map, 0, IntPtr.Zero, Marshal.SizeOf(typeof(GetChangesBuffer)), _getChangesBuffer);
            if (res > 0)
                return true;
            return false;
        }
        private void UnmapSharedBuffers()
        {
            int res = ExtEscape(_globalDC, UnMap, Marshal.SizeOf(typeof(GetChangesBuffer)), _getChangesBuffer, 0, IntPtr.Zero);
            if (res < 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Marshal.FreeHGlobal(_getChangesBuffer);
            _getChangesBuffer = IntPtr.Zero;

            ReleaseDC(IntPtr.Zero, _globalDC);
        }

        public Bitmap CaptureScreen()
        {
            if (State != MirrorState.Connected)
                throw new InvalidOperationException("In order to get current screen you must at least be connected to the driver");
            PixelFormat format;
            if (_bitmapBpp == 16)
                format = PixelFormat.Format16bppRgb565;
            else if (_bitmapBpp == 24)
                format = PixelFormat.Format24bppRgb;
            else if (_bitmapBpp == 32)
                format = PixelFormat.Format32bppArgb;
            else
            {
                Debug.Fail("Unknown pixel format");
                throw new Exception("Unknown pixel format");
            }
            var getChangesBuffer = (GetChangesBuffer)Marshal.PtrToStructure(_getChangesBuffer, typeof(GetChangesBuffer));
            return new Bitmap(_bitmapWidth, _bitmapHeight, _bitmapStride, format, getChangesBuffer.UserBuffer);
        }

        public byte[] MarshalScreenToByteArray()
        {
            if (State != MirrorState.Connected)
            {
                throw new InvalidOperationException("In order to get current screen you must at least be connected to the driver");
            }
            var getChangesBuffer = (GetChangesBuffer)Marshal.PtrToStructure(_getChangesBuffer, typeof(GetChangesBuffer));
            var data = new byte[_bitmapHeight* _bitmapStride];
            Marshal.Copy(getChangesBuffer.UserBuffer, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (State != MirrorState.Idle)
            {
                Unload();
            }
        }
    }

}
