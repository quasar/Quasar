namespace xServer.Core.Data
{
    public class RemoteDrive
    {
        public string DisplayName { get; private set; }

        public string RootDirectory { get; private set; }

        public RemoteDrive(string displayName, string rootDirectory)
        {
            this.DisplayName = displayName;
            this.RootDirectory = rootDirectory;
        }
    }
}
