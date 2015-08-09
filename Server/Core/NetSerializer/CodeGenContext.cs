/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace xServer.Core.NetSerializer
{
	public sealed class TypeData
	{
		public TypeData(ushort typeID, IDynamicTypeSerializer serializer)
		{
			this.TypeID = typeID;
			this.TypeSerializer = serializer;

			this.NeedsInstanceParameter = true;
		}

		public TypeData(ushort typeID, MethodInfo writer, MethodInfo reader)
		{
			this.TypeID = typeID;
			this.WriterMethodInfo = writer;
			this.ReaderMethodInfo = reader;

			this.NeedsInstanceParameter = writer.GetParameters().Length == 3;
		}

		public readonly ushort TypeID;
		public bool IsGenerated { get { return this.TypeSerializer != null; } }
		public readonly IDynamicTypeSerializer TypeSerializer;
		public MethodInfo WriterMethodInfo;
		public MethodInfo ReaderMethodInfo;

		public bool NeedsInstanceParameter { get; private set; }
	}

	public sealed class CodeGenContext
	{
		readonly Dictionary<Type, TypeData> m_typeMap;

		public CodeGenContext(Dictionary<Type, TypeData> typeMap)
		{
			m_typeMap = typeMap;

			var td = m_typeMap[typeof(object)];
			this.SerializerSwitchMethodInfo = td.WriterMethodInfo;
			this.DeserializerSwitchMethodInfo = td.ReaderMethodInfo;
		}

		public MethodInfo SerializerSwitchMethodInfo { get; private set; }
		public MethodInfo DeserializerSwitchMethodInfo { get; private set; }

		public MethodInfo GetWriterMethodInfo(Type type)
		{
			return m_typeMap[type].WriterMethodInfo;
		}

		public MethodInfo GetReaderMethodInfo(Type type)
		{
			return m_typeMap[type].ReaderMethodInfo;
		}

		public bool IsGenerated(Type type)
		{
			return m_typeMap[type].IsGenerated;
		}

		public IDictionary<Type, TypeData> TypeMap { get { return m_typeMap; } }

		bool CanCallDirect(Type type)
		{
			// We can call the (De)serializer method directly for:
			// - Value types
			// - Array types
			// - Sealed types with static (De)serializer method, as the method will handle null
			// Other reference types go through the (De)serializerSwitch

			bool direct;

			if (type.IsValueType || type.IsArray)
				direct = true;
			else if (type.IsSealed && IsGenerated(type) == false)
				direct = true;
			else
				direct = false;

			return direct;
		}

		public TypeData GetTypeData(Type type)
		{
			return m_typeMap[type];
		}

		public TypeData GetTypeDataForCall(Type type)
		{
			bool direct = CanCallDirect(type);
			if (!direct)
				type = typeof(object);

			return GetTypeData(type);
		}
	}
}
