using Microsoft.Win32;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class RegistryHandler : MessageProcessorBase<string>
    {
        /// <summary>
        /// The client which is associated with this registry handler.
        /// </summary>
        private readonly Client _client;

        public delegate void KeysReceivedEventHandler(object sender, string rootKey, RegSeekerMatch[] matches);
        public delegate void KeyCreatedEventHandler(object sender, string parentPath, RegSeekerMatch match);
        public delegate void KeyDeletedEventHandler(object sender, string parentPath, string subKey);
        public delegate void KeyRenamedEventHandler(object sender, string parentPath, string oldSubKey, string newSubKey);
        public delegate void ValueCreatedEventHandler(object sender, string keyPath, RegValueData value);
        public delegate void ValueDeletedEventHandler(object sender, string keyPath, string valueName);
        public delegate void ValueRenamedEventHandler(object sender, string keyPath, string oldValueName, string newValueName);
        public delegate void ValueChangedEventHandler(object sender, string keyPath, RegValueData value);

        public event KeysReceivedEventHandler KeysReceived;
        public event KeyCreatedEventHandler KeyCreated;
        public event KeyDeletedEventHandler KeyDeleted;
        public event KeyRenamedEventHandler KeyRenamed;
        public event ValueCreatedEventHandler ValueCreated;
        public event ValueDeletedEventHandler ValueDeleted;
        public event ValueRenamedEventHandler ValueRenamed;
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Reports initially received registry keys.
        /// </summary>
        /// <param name="rootKey">The root registry key name.</param>
        /// <param name="matches">The child registry keys.</param>
        private void OnKeysReceived(string rootKey, RegSeekerMatch[] matches)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = KeysReceived;
                handler?.Invoke(this, rootKey, (RegSeekerMatch[]) t);
            }, matches);
        }

        /// <summary>
        /// Reports created registry keys.
        /// </summary>
        /// <param name="parentPath">The registry key parent path.</param>
        /// <param name="match">The created registry key.</param>
        private void OnKeyCreated(string parentPath, RegSeekerMatch match)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = KeyCreated;
                handler?.Invoke(this, parentPath, (RegSeekerMatch) t);
            }, match);
        }

        /// <summary>
        /// Reports deleted registry keys.
        /// </summary>
        /// <param name="parentPath">The registry key parent path.</param>
        /// <param name="subKey">The registry sub key name.</param>
        private void OnKeyDeleted(string parentPath, string subKey)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = KeyDeleted;
                handler?.Invoke(this, parentPath, (string) t);
            }, subKey);
        }

        /// <summary>
        /// Reports renamed registry keys.
        /// </summary>
        /// <param name="parentPath">The registry key parent path.</param>
        /// <param name="oldSubKey">The old registry sub key name.</param>
        /// <param name="newSubKey">The new registry sub key name.</param>
        private void OnKeyRenamed(string parentPath, string oldSubKey, string newSubKey)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = KeyRenamed;
                handler?.Invoke(this, parentPath, oldSubKey, (string) t);
            }, newSubKey);
        }

        /// <summary>
        /// Reports created registry values.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="value">The created value.</param>
        private void OnValueCreated(string keyPath, RegValueData value)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = ValueCreated;
                handler?.Invoke(this, keyPath, (RegValueData)t);
            }, value);
        }

        /// <summary>
        /// Reports deleted registry values.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="valueName">The value name.</param>
        private void OnValueDeleted(string keyPath, string valueName)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = ValueDeleted;
                handler?.Invoke(this, keyPath, (string) t);
            }, valueName);
        }

        /// <summary>
        /// Reports renamed registry values.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="oldValueName">The old value name.</param>
        /// <param name="newValueName">The new value name.</param>
        private void OnValueRenamed(string keyPath, string oldValueName, string newValueName)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = ValueRenamed;
                handler?.Invoke(this, keyPath, oldValueName, (string) t);
            }, newValueName);
        }

        /// <summary>
        /// Reports changed registry values.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="value">The new value.</param>
        private void OnValueChanged(string keyPath, RegValueData value)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = ValueChanged;
                handler?.Invoke(this, keyPath, (RegValueData) t);
            }, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public RegistryHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetRegistryKeysResponse ||
                                                             message is GetCreateRegistryKeyResponse ||
                                                             message is GetDeleteRegistryKeyResponse ||
                                                             message is GetRenameRegistryKeyResponse ||
                                                             message is GetCreateRegistryValueResponse ||
                                                             message is GetDeleteRegistryValueResponse ||
                                                             message is GetRenameRegistryValueResponse ||
                                                             message is GetChangeRegistryValueResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetRegistryKeysResponse keysResp:
                    Execute(sender, keysResp);
                    break;
                case GetCreateRegistryKeyResponse createKeysResp:
                    Execute(sender, createKeysResp);
                    break;
                case GetDeleteRegistryKeyResponse deleteKeysResp:
                    Execute(sender, deleteKeysResp);
                    break;
                case GetRenameRegistryKeyResponse renameKeysResp:
                    Execute(sender, renameKeysResp);
                    break;
                case GetCreateRegistryValueResponse createValueResp:
                    Execute(sender, createValueResp);
                    break;
                case GetDeleteRegistryValueResponse deleteValueResp:
                    Execute(sender, deleteValueResp);
                    break;
                case GetRenameRegistryValueResponse renameValueResp:
                    Execute(sender, renameValueResp);
                    break;
                case GetChangeRegistryValueResponse changeValueResp:
                    Execute(sender, changeValueResp);
                    break;
            }
        }

        /// <summary>
        /// Loads the registry keys of a given root key.
        /// </summary>
        /// <param name="rootKeyName">The root key name.</param>
        public void LoadRegistryKey(string rootKeyName)
        {
            _client.Send(new DoLoadRegistryKey
            {
                RootKeyName = rootKeyName
            });
        }

        /// <summary>
        /// Creates a registry key at the given parent path.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        public void CreateRegistryKey(string parentPath)
        {
            _client.Send(new DoCreateRegistryKey
            {
                ParentPath = parentPath
            });
        }

        /// <summary>
        /// Deletes the given registry key.
        /// </summary>
        /// <param name="parentPath">The parent path of the registry key to delete.</param>
        /// <param name="keyName">The registry key name to delete.</param>
        public void DeleteRegistryKey(string parentPath, string keyName)
        {
            _client.Send(new DoDeleteRegistryKey
            {
                ParentPath = parentPath,
                KeyName = keyName
            });
        }

        /// <summary>
        /// Renames the given registry key.
        /// </summary>
        /// <param name="parentPath">The parent path of the registry key to rename.</param>
        /// <param name="oldKeyName">The old name of the registry key.</param>
        /// <param name="newKeyName">The new name of the registry key.</param>
        public void RenameRegistryKey(string parentPath, string oldKeyName, string newKeyName)
        {
            _client.Send(new DoRenameRegistryKey
            {
                ParentPath = parentPath,
                OldKeyName = oldKeyName,
                NewKeyName = newKeyName
            });
        }

        /// <summary>
        /// Creates a registry key value.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="kind">The kind of registry key value.</param>
        public void CreateRegistryValue(string keyPath, RegistryValueKind kind)
        {
            _client.Send(new DoCreateRegistryValue
            {
                KeyPath = keyPath,
                Kind = kind
            });
        }

        /// <summary>
        /// Deletes the registry key value.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="valueName">The registry key value name to delete.</param>
        public void DeleteRegistryValue(string keyPath, string valueName)
        {
            _client.Send(new DoDeleteRegistryValue
            {
                KeyPath = keyPath,
                ValueName = valueName
            });
        }

        /// <summary>
        /// Renames the registry key value.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="oldValueName">The old registry key value name.</param>
        /// <param name="newValueName">The new registry key value name.</param>
        public void RenameRegistryValue(string keyPath, string oldValueName, string newValueName)
        {
            _client.Send(new DoRenameRegistryValue
            {
                KeyPath = keyPath,
                OldValueName = oldValueName,
                NewValueName = newValueName
            });
        }

        /// <summary>
        /// Changes the registry key value.
        /// </summary>
        /// <param name="keyPath">The registry key path.</param>
        /// <param name="value">The updated registry key value.</param>
        public void ChangeRegistryValue(string keyPath, RegValueData value)
        {
            _client.Send(new DoChangeRegistryValue
            {
                KeyPath = keyPath,
                Value = value
            });
        }

        private void Execute(ISender client, GetRegistryKeysResponse message)
        {
            if (!message.IsError)
            {
                OnKeysReceived(message.RootKey, message.Matches);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetCreateRegistryKeyResponse message)
        {
            if (!message.IsError)
            {
                OnKeyCreated(message.ParentPath, message.Match);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetDeleteRegistryKeyResponse message)
        {
            if (!message.IsError)
            {
                OnKeyDeleted(message.ParentPath, message.KeyName);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetRenameRegistryKeyResponse message)
        {
            if (!message.IsError)
            {
                OnKeyRenamed(message.ParentPath, message.OldKeyName, message.NewKeyName);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetCreateRegistryValueResponse message)
        {
            if (!message.IsError)
            {
                OnValueCreated(message.KeyPath, message.Value);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetDeleteRegistryValueResponse message)
        {
            if (!message.IsError)
            {
                OnValueDeleted(message.KeyPath, message.ValueName);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetRenameRegistryValueResponse message)
        {
            if (!message.IsError)
            {
                OnValueRenamed(message.KeyPath, message.OldValueName, message.NewValueName);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        private void Execute(ISender client, GetChangeRegistryValueResponse message)
        {
            if (!message.IsError)
            {
                OnValueChanged(message.KeyPath, message.Value);
            }
            else
            {
                OnReport(message.ErrorMsg);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
