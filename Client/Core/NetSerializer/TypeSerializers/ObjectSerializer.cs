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
using System.Reflection.Emit;

namespace xClient.Core.NetSerializer.TypeSerializers
{
	class ObjectSerializer : IDynamicTypeSerializer
	{
		public bool Handles(Type type)
		{
			return type == typeof(object);
		}

		public IEnumerable<Type> GetSubtypes(Type type)
		{
			return new Type[0];
		}

		public void GenerateWriterMethod(Type obtype, CodeGenContext ctx, ILGenerator il)
		{
			var getTypeIDMethodInfo = typeof(Serializer).GetMethod("GetTypeID", BindingFlags.NonPublic | BindingFlags.Instance, null,
				new Type[] { typeof(object) }, null);

			var map = ctx.TypeMap;

			// arg0: Serializer, arg1: Stream, arg2: object

			var idLocal = il.DeclareLocal(typeof(ushort));

			// get TypeID from object's Type
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, getTypeIDMethodInfo);
			il.Emit(OpCodes.Stloc_S, idLocal);

			// write typeID
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloc_S, idLocal);
			il.Emit(OpCodes.Call, ctx.GetWriterMethodInfo(typeof(ushort)));

			// +1 for 0 (null)
			var jumpTable = new Label[map.Count + 1];
			jumpTable[0] = il.DefineLabel();
			foreach (var kvp in map)
				jumpTable[kvp.Value.TypeID] = il.DefineLabel();

			il.Emit(OpCodes.Ldloc_S, idLocal);
			il.Emit(OpCodes.Switch, jumpTable);

			il.Emit(OpCodes.Newobj, Helpers.ExceptionCtorInfo);
			il.Emit(OpCodes.Throw);

			/* null case */
			il.MarkLabel(jumpTable[0]);
			il.Emit(OpCodes.Ret);

			/* cases for types */
			foreach (var kvp in map)
			{
				var type = kvp.Key;
				var data = kvp.Value;

				il.MarkLabel(jumpTable[data.TypeID]);

				if (data.NeedsInstanceParameter)
					il.Emit(OpCodes.Ldarg_0);

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);

				il.Emit(OpCodes.Tailcall);
				il.Emit(OpCodes.Call, data.WriterMethodInfo);

				il.Emit(OpCodes.Ret);
			}
		}

		public void GenerateReaderMethod(Type obtype, CodeGenContext ctx, ILGenerator il)
		{
			var map = ctx.TypeMap;

			// arg0: Serializer, arg1: stream, arg2: out object

			var idLocal = il.DeclareLocal(typeof(ushort));

			// read typeID
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloca_S, idLocal);
			il.Emit(OpCodes.Call, ctx.GetReaderMethodInfo(typeof(ushort)));

			// +1 for 0 (null)
			var jumpTable = new Label[map.Count + 1];
			jumpTable[0] = il.DefineLabel();
			foreach (var kvp in map)
				jumpTable[kvp.Value.TypeID] = il.DefineLabel();

			il.Emit(OpCodes.Ldloc_S, idLocal);
			il.Emit(OpCodes.Switch, jumpTable);

			il.Emit(OpCodes.Newobj, Helpers.ExceptionCtorInfo);
			il.Emit(OpCodes.Throw);

			/* null case */
			il.MarkLabel(jumpTable[0]);

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Stind_Ref);
			il.Emit(OpCodes.Ret);

			/* cases for types */
			foreach (var kvp in map)
			{
				var type = kvp.Key;
				var data = kvp.Value;

				il.MarkLabel(jumpTable[data.TypeID]);

				var local = il.DeclareLocal(type);

				// call deserializer for this typeID
				if (data.NeedsInstanceParameter)
					il.Emit(OpCodes.Ldarg_0);

				il.Emit(OpCodes.Ldarg_1);
				if (local.LocalIndex < 256)
					il.Emit(OpCodes.Ldloca_S, local);
				else
					il.Emit(OpCodes.Ldloca, local);

				il.Emit(OpCodes.Call, data.ReaderMethodInfo);

				// write result object to out object
				il.Emit(OpCodes.Ldarg_2);
				if (local.LocalIndex < 256)
					il.Emit(OpCodes.Ldloc_S, local);
				else
					il.Emit(OpCodes.Ldloc, local);
				if (type.IsValueType)
					il.Emit(OpCodes.Box, type);
				il.Emit(OpCodes.Stind_Ref);

				il.Emit(OpCodes.Ret);
			}
		}
	}
}
