using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace xServer.Core.Build
{
    public class Renamer
    {
        public AssemblyDefinition AsmDef { get; set; }

        private int length { get; set; }
        private MemberOverloader TypeOverloader;
        private Dictionary<TypeDefinition, MemberOverloader> methodOverloaders;
        private Dictionary<TypeDefinition, MemberOverloader> fieldOverloaders;
        private Dictionary<TypeDefinition, MemberOverloader> eventOverloaders;

        public Renamer(AssemblyDefinition asmDef)
            : this(asmDef, 20)
        {
        }

        public Renamer(AssemblyDefinition asmDef, int length)
        {
            this.AsmDef = asmDef;
            this.length = length;
            TypeOverloader = new MemberOverloader(this.length);
            methodOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            fieldOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            eventOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
        }

        public bool Perform()
        {
            try
            {
                foreach (var module in AsmDef.Modules)
                {
                    foreach (TypeDefinition typeDef in module.Types)
                    {
                        RenameInType(typeDef);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RenameInType(TypeDefinition typeDef)
        {
            if (typeDef.Namespace.StartsWith("My") || typeDef.Namespace.StartsWith("xClient.Core.Packets") ||
                typeDef.Namespace == "xClient.Core" || typeDef.Namespace == "xClient.Core.Elevation" ||
                typeDef.Namespace == "xClient.Core.Compression" || typeDef.Namespace.StartsWith("ProtoBuf"))
                return;

            TypeOverloader.GiveName(typeDef);

            typeDef.Namespace = string.Empty;

            MemberOverloader methodOverloader = GetMethodOverloader(typeDef);
            MemberOverloader fieldOverloader = GetFieldOverloader(typeDef);
            MemberOverloader eventOverloader = GetEventOverloader(typeDef);

            if (typeDef.HasNestedTypes)
                foreach (TypeDefinition nestedType in typeDef.NestedTypes)
                    RenameInType(nestedType);

            if (typeDef.HasMethods)
                foreach (MethodDefinition methodDef in typeDef.Methods)
                    if (!methodDef.IsConstructor)
                    {
                        methodOverloader.GiveName(methodDef);
                    }

            if (typeDef.HasFields)
                foreach (FieldDefinition fieldDef in typeDef.Fields)
                    fieldOverloader.GiveName(fieldDef);


            if (typeDef.HasEvents)
                foreach (EventDefinition eventDef in typeDef.Events)
                    eventOverloader.GiveName(eventDef);
        }

        private MemberOverloader GetMethodOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this.methodOverloaders, typeDef);
        }

        private MemberOverloader GetFieldOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this.fieldOverloaders, typeDef);
        }

        private MemberOverloader GetEventOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this.eventOverloaders, typeDef);
        }

        private MemberOverloader GetOverloader(Dictionary<TypeDefinition, MemberOverloader> overloaderDictionary,
            TypeDefinition targetTypeDef)
        {
            MemberOverloader overloader;
            if (!overloaderDictionary.TryGetValue(targetTypeDef, out overloader))
            {
                overloader = new MemberOverloader(this.length);
                overloaderDictionary.Add(targetTypeDef, overloader);
            }
            return overloader;
        }

        private class MemberOverloader
        {
            private bool DoRandom { get; set; }
            private int StartingLength { get; set; }
            private Dictionary<string, string> RenamedMembers = new Dictionary<string, string>();
            private char[] CharMap;
            private int[] Indices;

            public MemberOverloader(int startingLength, bool doRandom = true)
                : this(startingLength, doRandom, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray())
            {
            }

            public MemberOverloader(int startingLength, bool doRandom, char[] chars)
            {
                this.CharMap = chars;
                this.DoRandom = doRandom;
                this.StartingLength = startingLength;
                this.Indices = new int[startingLength];
            }

            public void GiveName(MemberReference member)
            {
                string currentName = GetCurrentName();
                string originalName = member.ToString();
                member.Name = currentName;
                while (RenamedMembers.ContainsValue(member.ToString()))
                {
                    member.Name = GetCurrentName();
                }
                RenamedMembers.Add(originalName, member.ToString());
            }

            private string GetCurrentName()
            {
                if (DoRandom)
                    return GetRandomName();
                else
                    return GetOverloadedName();
            }

            private string GetRandomName()
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < StartingLength; i++)
                {
                    builder.Append((char) new Random(Guid.NewGuid().GetHashCode()).Next(int.MinValue, int.MaxValue));
                }

                return builder.ToString();
            }

            private string GetOverloadedName()
            {
                IncrementIndices();
                char[] chars = new char[Indices.Length];
                for (int i = 0; i < Indices.Length; i++)
                    chars[i] = CharMap[Indices[i]];
                return new string(chars);
            }

            private void IncrementIndices()
            {
                for (int i = Indices.Length - 1; i >= 0; i--)
                {
                    Indices[i]++;
                    if (Indices[i] >= CharMap.Length)
                    {
                        if (i == 0)
                            Array.Resize(ref Indices, Indices.Length + 1);
                        Indices[i] = 0;
                    }
                    else
                        break;
                }
            }
        }
    }
}