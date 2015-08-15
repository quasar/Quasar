/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace xServer.Core.NetSerializer.TypeSerializers
{
	public class DictionarySerializer : IStaticTypeSerializer
	{
		public bool Handles(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var genTypeDef = type.GetGenericTypeDefinition();

			return genTypeDef == typeof(Dictionary<,>);
		}

		public IEnumerable<Type> GetSubtypes(Type type)
		{
			// Dictionary<K,V> is stored as KeyValuePair<K,V>[]

			var genArgs = type.GetGenericArguments();

			var serializedType = typeof(KeyValuePair<,>).MakeGenericType(genArgs).MakeArrayType();

			yield return serializedType;
		}

		public void GetStaticMethods(Type type, out MethodInfo writer, out MethodInfo reader)
		{
			Debug.Assert(type.IsGenericType);

			if (!type.IsGenericType)
				throw new Exception();

			var genTypeDef = type.GetGenericTypeDefinition();

			Debug.Assert(genTypeDef == typeof(Dictionary<,>));

			var containerType = this.GetType();

			writer = GetGenWriter(containerType, genTypeDef);
			reader = GetGenReader(containerType, genTypeDef);

			var genArgs = type.GetGenericArguments();

			writer = writer.MakeGenericMethod(genArgs);
			reader = reader.MakeGenericMethod(genArgs);
		}

		static MethodInfo GetGenWriter(Type containerType, Type genType)
		{
			var mis = containerType.GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi => mi.IsGenericMethod && mi.Name == "WritePrimitive");

			foreach (var mi in mis)
			{
				var p = mi.GetParameters();

				if (p.Length != 3)
					continue;

				if (p[1].ParameterType != typeof(Stream))
					continue;

				var paramType = p[2].ParameterType;

				if (paramType.IsGenericType == false)
					continue;

				var genParamType = paramType.GetGenericTypeDefinition();

				if (genType == genParamType)
					return mi;
			}

			return null;
		}

		static MethodInfo GetGenReader(Type containerType, Type genType)
		{
			var mis = containerType.GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(mi => mi.IsGenericMethod && mi.Name == "ReadPrimitive");

			foreach (var mi in mis)
			{
				var p = mi.GetParameters();

				if (p.Length != 3)
					continue;

				if (p[1].ParameterType != typeof(Stream))
					continue;

				var paramType = p[2].ParameterType;

				if (paramType.IsByRef == false)
					continue;

				paramType = paramType.GetElementType();

				if (paramType.IsGenericType == false)
					continue;

				var genParamType = paramType.GetGenericTypeDefinition();

				if (genType == genParamType)
					return mi;
			}

			return null;
		}

		public static void WritePrimitive<TKey, TValue>(Serializer serializer, Stream stream, Dictionary<TKey, TValue> value)
		{
			var kvpArray = new KeyValuePair<TKey, TValue>[value.Count];

			int i = 0;
			foreach (var kvp in value)
				kvpArray[i++] = kvp;

			serializer.Serialize(stream, kvpArray);
		}

		public static void ReadPrimitive<TKey, TValue>(Serializer serializer, Stream stream, out Dictionary<TKey, TValue> value)
		{
			var kvpArray = (KeyValuePair<TKey, TValue>[])serializer.Deserialize(stream);

			value = new Dictionary<TKey, TValue>(kvpArray.Length);

			foreach (var kvp in kvpArray)
				value.Add(kvp.Key, kvp.Value);
		}
	}
}
