using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace xServer.Core.NAudio.Wave.MmeInterop
{
    class WaveWindowNative : System.Windows.Forms.NativeWindow
    {
        private WaveInterop.WaveCallback waveCallback;

        public WaveWindowNative(WaveInterop.WaveCallback waveCallback)
        {
            this.waveCallback = waveCallback;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            WaveInterop.WaveMessage message = (WaveInterop.WaveMessage)m.Msg;
            
            switch(message)
            {
                case WaveInterop.WaveMessage.WaveOutDone:
                case WaveInterop.WaveMessage.WaveInData:
                    IntPtr hOutputDevice = m.WParam;
                    WaveHeader waveHeader = new WaveHeader();
                    Marshal.PtrToStructure(m.LParam, waveHeader);
                    waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                    break;
                case WaveInterop.WaveMessage.WaveOutOpen:
                case WaveInterop.WaveMessage.WaveOutClose:
                case WaveInterop.WaveMessage.WaveInClose:
                case WaveInterop.WaveMessage.WaveInOpen:
                    waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }

    class WaveWindow : Form
    {
        private WaveInterop.WaveCallback waveCallback;

        public WaveWindow(WaveInterop.WaveCallback waveCallback)
        {
            this.waveCallback = waveCallback;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            WaveInterop.WaveMessage message = (WaveInterop.WaveMessage)m.Msg;
            
            switch(message)
            {
                case WaveInterop.WaveMessage.WaveOutDone:
                case WaveInterop.WaveMessage.WaveInData:
                    IntPtr hOutputDevice = m.WParam;
                    WaveHeader waveHeader = new WaveHeader();
                    Marshal.PtrToStructure(m.LParam, waveHeader);
                    waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                    break;
                case WaveInterop.WaveMessage.WaveOutOpen:
                case WaveInterop.WaveMessage.WaveOutClose:
                case WaveInterop.WaveMessage.WaveInClose:
                case WaveInterop.WaveMessage.WaveInOpen:
                    waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
