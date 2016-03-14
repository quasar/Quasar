using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace xServer.Core.Build
{
    public class Renamer
    {
        /// <summary>
        /// Contains the assembly definition.
        /// </summary>
        public AssemblyDefinition AsmDef { get; set; }

        private int Length { get; set; }
        private MemberOverloader _typeOverloader;
        private Dictionary<TypeDefinition, MemberOverloader> _methodOverloaders;
        private Dictionary<TypeDefinition, MemberOverloader> _fieldOverloaders;
        private Dictionary<TypeDefinition, MemberOverloader> _eventOverloaders;

        public Renamer(AssemblyDefinition asmDef)
            : this(asmDef, 20)
        {
        }

        public Renamer(AssemblyDefinition asmDef, int length)
        {
            this.AsmDef = asmDef;
            this.Length = length;
            _typeOverloader = new MemberOverloader(this.Length);
            _methodOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            _fieldOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
            _eventOverloaders = new Dictionary<TypeDefinition, MemberOverloader>();
        }

        /// <summary>
        /// Attempts to modify the assembly definition data.
        /// </summary>
        /// <returns>True if the operation succeeded; False if the operation failed.</returns>
        public bool Perform()
        {
            try
            {
                foreach (TypeDefinition typeDef in AsmDef.Modules.SelectMany(module => module.Types))
                {
                    RenameInType(typeDef);
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
            if (typeDef.Namespace.Contains("NetSerializer") 
                || typeDef.Namespace.Contains("Registry") || typeDef.HasInterfaces)
                return;

            _typeOverloader.GiveName(typeDef);

            typeDef.Namespace = string.Empty;

            MemberOverloader methodOverloader = GetMethodOverloader(typeDef);
            MemberOverloader fieldOverloader = GetFieldOverloader(typeDef);
            MemberOverloader eventOverloader = GetEventOverloader(typeDef);

            if (typeDef.HasNestedTypes)
                foreach (TypeDefinition nestedType in typeDef.NestedTypes)
                    RenameInType(nestedType);

            if (typeDef.HasMethods)
                foreach (MethodDefinition methodDef in
                        typeDef.Methods.Where(methodDef =>
                                !methodDef.IsConstructor && !methodDef.HasCustomAttributes &&
                                !methodDef.IsAbstract && !methodDef.IsVirtual))
                    methodOverloader.GiveName(methodDef);

            if (typeDef.HasFields)
                foreach (FieldDefinition fieldDef in typeDef.Fields)
                    fieldOverloader.GiveName(fieldDef);

            if (typeDef.HasEvents)
                foreach (EventDefinition eventDef in typeDef.Events)
                    eventOverloader.GiveName(eventDef);
        }

        private MemberOverloader GetMethodOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this._methodOverloaders, typeDef);
        }

        private MemberOverloader GetFieldOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this._fieldOverloaders, typeDef);
        }

        private MemberOverloader GetEventOverloader(TypeDefinition typeDef)
        {
            return GetOverloader(this._eventOverloaders, typeDef);
        }

        private MemberOverloader GetOverloader(Dictionary<TypeDefinition, MemberOverloader> overloaderDictionary,
            TypeDefinition targetTypeDef)
        {
            MemberOverloader overloader;
            if (!overloaderDictionary.TryGetValue(targetTypeDef, out overloader))
            {
                overloader = new MemberOverloader(this.Length);
                overloaderDictionary.Add(targetTypeDef, overloader);
            }
            return overloader;
        }

        private class MemberOverloader
        {
            private bool DoRandom { get; set; }
            private int StartingLength { get; set; }
            private Dictionary<string, string> _renamedMembers = new Dictionary<string, string>();
            private readonly char[] _charMap;
            private int[] _indices;

            public MemberOverloader(int startingLength, bool doRandom = true)
                : this(startingLength, doRandom, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray())
            {
            }

            private MemberOverloader(int startingLength, bool doRandom, char[] chars)
            {
                this._charMap = chars;
                this.DoRandom = doRandom;
                this.StartingLength = startingLength;
                this._indices = new int[startingLength];
            }

            public void GiveName(MemberReference member)
            {
                string currentName = GetCurrentName();
                string originalName = member.ToString();
                member.Name = currentName;
                while (_renamedMembers.ContainsValue(member.ToString()))
                {
                    member.Name = GetCurrentName();
                }
                _renamedMembers.Add(originalName, member.ToString());
            }

            private string GetCurrentName()
            {
                return DoRandom ? GetRandomName() : GetOverloadedName();
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
                char[] chars = new char[_indices.Length];
                for (int i = 0; i < _indices.Length; i++)
                    chars[i] = _charMap[_indices[i]];
                return new string(chars);
            }

            private void IncrementIndices()
            {
                for (int i = _indices.Length - 1; i >= 0; i--)
                {
                    _indices[i]++;
                    if (_indices[i] >= _charMap.Length)
                    {
                        if (i == 0)
                            Array.Resize(ref _indices, _indices.Length + 1);
                        _indices[i] = 0;
                    }
                    else
                        break;
                }
            }
        }
    }
}