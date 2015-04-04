using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace xServer.Core.Build
{
    public class Renamer
    {
        private readonly Dictionary<TypeDefinition, MemberOverloader> eventOverloaders;
        private readonly Dictionary<TypeDefinition, MemberOverloader> fieldOverloaders;
        private readonly Dictionary<TypeDefinition, MemberOverloader> methodOverloaders;
        private readonly MemberOverloader TypeOverloader;

        public Renamer(AssemblyDefinition asmDef)
            : this(asmDef, 20)
        {
        }

        public Renamer(AssemblyDefinition asmDef, int length)
        {
            AsmDef = asmDef;
            this.length = length;
            TypeOverloader = new MemberOverloader(this.length);
            methodOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            fieldOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            eventOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
        }

        public AssemblyDefinition AsmDef { get; set; }
        private int length { get; set; }

        public bool Perform()
        {
            try
            {
                foreach (var module in AsmDef.Modules)
                {
                    foreach (var typeDef in module.Types)
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

            var methodOverloader = GetMethodOverloader(typeDef);
            var fieldOverloader = GetFieldOverloader(typeDef);
            var eventOverloader = GetEventOverloader(typeDef);

            if (typeDef.HasNestedTypes)
                foreach (var nestedType in typeDef.NestedTypes)
                    RenameInType(nestedType);

            if (typeDef.HasMethods)
                foreach (var methodDef in typeDef.Methods)
                    if (!methodDef.IsConstructor)
                    {
                        methodOverloader.GiveName(methodDef);
                    }

            if (typeDef.HasFields)
                foreach (var fieldDef in typeDef.Fields)
                    fieldOverloader.GiveName(fieldDef);

            if (typeDef.HasEvents)
                foreach (var eventDef in typeDef.Events)
                    eventOverloader.GiveName(eventDef);
        }

        private MemberOverloader GetMethodOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(methodOverloaders, typeDef);
        }

        private MemberOverloader GetFieldOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(fieldOverloaders, typeDef);
        }

        private MemberOverloader GetEventOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(eventOverloaders, typeDef);
        }

        private MemberOverloader GetOverloader(Dictionary<TypeDefinition, MemberOverloader> overloaderDictionary,
            TypeDefinition targetTypeDef)
        {
            MemberOverloader overloader;
            if (!overloaderDictionary.TryGetValue(targetTypeDef, out overloader))
            {
                overloader = new MemberOverloader(length);
                overloaderDictionary.Add(targetTypeDef, overloader);
            }
            return overloader;
        }

        private class MemberOverloader
        {
            private readonly char[] CharMap;
            private readonly Dictionary<string, string> RenamedMembers = new Dictionary<string, string>();
            private int[] Indices;

            public MemberOverloader(int startingLength, bool doRandom = true)
                : this(startingLength, doRandom, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray())
            {
            }

            public MemberOverloader(int startingLength, bool doRandom, char[] chars)
            {
                CharMap = chars;
                DoRandom = doRandom;
                StartingLength = startingLength;
                Indices = new int[startingLength];
            }

            private bool DoRandom { get; set; }
            private int StartingLength { get; set; }

            public void GiveName(MemberReference member)
            {
                var currentName = GetCurrentName();
                var originalName = member.ToString();
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
                return GetOverloadedName();
            }

            private string GetRandomName()
            {
                var builder = new StringBuilder();

                for (var i = 0; i < StartingLength; i++)
                {
                    builder.Append((char) new Random(Guid.NewGuid().GetHashCode()).Next(int.MinValue, int.MaxValue));
                }

                return builder.ToString();
            }

            private string GetOverloadedName()
            {
                IncrementIndices();
                var chars = new char[Indices.Length];
                for (var i = 0; i < Indices.Length; i++)
                    chars[i] = CharMap[Indices[i]];
                return new string(chars);
            }

            private void IncrementIndices()
            {
                for (var i = Indices.Length - 1; i >= 0; i--)
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