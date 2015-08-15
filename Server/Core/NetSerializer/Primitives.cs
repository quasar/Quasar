/*
 * Copyright 2015 Tomi Valkeinen
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace xServer.Core.NetSerializer
{
	public static class Primitives
	{
		public static MethodInfo GetWritePrimitive(Type type)
		{
			return typeof(Primitives).GetMethod("WritePrimitive",
				BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null,
				new Type[] { typeof(Stream), type }, null);
		}

		public static MethodInfo GetReaderPrimitive(Type type)
		{
			return typeof(Primitives).GetMethod("ReadPrimitive",
				BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null,
				new Type[] { typeof(Stream), type.MakeByRefType() }, null);
		}

		static uint EncodeZigZag32(int n)
		{
			return (uint)((n << 1) ^ (n >> 31));
		}

		static ulong EncodeZigZag64(long n)
		{
			return (ulong)((n << 1) ^ (n >> 63));
		}

		static int DecodeZigZag32(uint n)
		{
			return (int)(n >> 1) ^ -(int)(n & 1);
		}

		static long DecodeZigZag64(ulong n)
		{
			return (long)(n >> 1) ^ -(long)(n & 1);
		}

		static uint ReadVarint32(Stream stream)
		{
			int result = 0;
			int offset = 0;

			for (; offset < 32; offset += 7)
			{
				int b = stream.ReadByte();
				if (b == -1)
					throw new EndOfStreamException();

				result |= (b & 0x7f) << offset;

				if ((b & 0x80) == 0)
					return (uint)result;
			}

			throw new InvalidDataException();
		}

		static void WriteVarint32(Stream stream, uint value)
		{
			for (; value >= 0x80u; value >>= 7)
				stream.WriteByte((byte)(value | 0x80u));

			stream.WriteByte((byte)value);
		}

		static ulong ReadVarint64(Stream stream)
		{
			long result = 0;
			int offset = 0;

			for (; offset < 64; offset += 7)
			{
				int b = stream.ReadByte();
				if (b == -1)
					throw new EndOfStreamException();

				result |= ((long)(b & 0x7f)) << offset;

				if ((b & 0x80) == 0)
					return (ulong)result;
			}

			throw new InvalidDataException();
		}

		static void WriteVarint64(Stream stream, ulong value)
		{
			for (; value >= 0x80u; value >>= 7)
				stream.WriteByte((byte)(value | 0x80u));

			stream.WriteByte((byte)value);
		}


		public static void WritePrimitive(Stream stream, bool value)
		{
			stream.WriteByte(value ? (byte)1 : (byte)0);
		}

		public static void ReadPrimitive(Stream stream, out bool value)
		{
			var b = stream.ReadByte();
			value = b != 0;
		}

		public static void WritePrimitive(Stream stream, byte value)
		{
			stream.WriteByte(value);
		}

		public static void ReadPrimitive(Stream stream, out byte value)
		{
			value = (byte)stream.ReadByte();
		}

		public static void WritePrimitive(Stream stream, sbyte value)
		{
			stream.WriteByte((byte)value);
		}

		public static void ReadPrimitive(Stream stream, out sbyte value)
		{
			value = (sbyte)stream.ReadByte();
		}

		public static void WritePrimitive(Stream stream, char value)
		{
			WriteVarint32(stream, value);
		}

		public static void ReadPrimitive(Stream stream, out char value)
		{
			value = (char)ReadVarint32(stream);
		}

		public static void WritePrimitive(Stream stream, ushort value)
		{
			WriteVarint32(stream, value);
		}

		public static void ReadPrimitive(Stream stream, out ushort value)
		{
			value = (ushort)ReadVarint32(stream);
		}

		public static void WritePrimitive(Stream stream, short value)
		{
			WriteVarint32(stream, EncodeZigZag32(value));
		}

		public static void ReadPrimitive(Stream stream, out short value)
		{
			value = (short)DecodeZigZag32(ReadVarint32(stream));
		}

		public static void WritePrimitive(Stream stream, uint value)
		{
			WriteVarint32(stream, value);
		}

		public static void ReadPrimitive(Stream stream, out uint value)
		{
			value = ReadVarint32(stream);
		}

		public static void WritePrimitive(Stream stream, int value)
		{
			WriteVarint32(stream, EncodeZigZag32(value));
		}

		public static void ReadPrimitive(Stream stream, out int value)
		{
			value = DecodeZigZag32(ReadVarint32(stream));
		}

		public static void WritePrimitive(Stream stream, ulong value)
		{
			WriteVarint64(stream, value);
		}

		public static void ReadPrimitive(Stream stream, out ulong value)
		{
			value = ReadVarint64(stream);
		}

		public static void WritePrimitive(Stream stream, long value)
		{
			WriteVarint64(stream, EncodeZigZag64(value));
		}

		public static void ReadPrimitive(Stream stream, out long value)
		{
			value = DecodeZigZag64(ReadVarint64(stream));
		}

#if !NO_UNSAFE
		public static unsafe void WritePrimitive(Stream stream, float value)
		{
			uint v = *(uint*)(&value);
			WriteVarint32(stream, v);
		}

		public static unsafe void ReadPrimitive(Stream stream, out float value)
		{
			uint v = ReadVarint32(stream);
			value = *(float*)(&v);
		}

		public static unsafe void WritePrimitive(Stream stream, double value)
		{
			ulong v = *(ulong*)(&value);
			WriteVarint64(stream, v);
		}

		public static unsafe void ReadPrimitive(Stream stream, out double value)
		{
			ulong v = ReadVarint64(stream);
			value = *(double*)(&v);
		}
#else
		public static void WritePrimitive(Stream stream, float value)
		{
			WritePrimitive(stream, (double)value);
		}

		public static void ReadPrimitive(Stream stream, out float value)
		{
			double v;
			ReadPrimitive(stream, out v);
			value = (float)v;
		}

		public static void WritePrimitive(Stream stream, double value)
		{
			ulong v = (ulong)BitConverter.DoubleToInt64Bits(value);
			WriteVarint64(stream, v);
		}

		public static void ReadPrimitive(Stream stream, out double value)
		{
			ulong v = ReadVarint64(stream);
			value = BitConverter.Int64BitsToDouble((long)v);
		}
#endif

		public static void WritePrimitive(Stream stream, DateTime value)
		{
			long v = value.ToBinary();
			WritePrimitive(stream, v);
		}

		public static void ReadPrimitive(Stream stream, out DateTime value)
		{
			long v;
			ReadPrimitive(stream, out v);
			value = DateTime.FromBinary(v);
		}

#if NO_UNSAFE
		public static void WritePrimitive(Stream stream, string value)
		{
			if (value == null)
			{
				WritePrimitive(stream, (uint)0);
				return;
			}

			var encoding = new UTF8Encoding(false, true);

			int len = encoding.GetByteCount(value);

			WritePrimitive(stream, (uint)len + 1);

			var buf = new byte[len];

			encoding.GetBytes(value, 0, value.Length, buf, 0);

			stream.Write(buf, 0, len);
		}

		public static void ReadPrimitive(Stream stream, out string value)
		{
			uint len;
			ReadPrimitive(stream, out len);

			if (len == 0)
			{
				value = null;
				return;
			}
			else if (len == 1)
			{
				value = string.Empty;
				return;
			}

			len -= 1;

			var encoding = new UTF8Encoding(false, true);

			var buf = new byte[len];

			int l = 0;

			while (l < len)
			{
				int r = stream.Read(buf, l, (int)len - l);
				if (r == 0)
					throw new EndOfStreamException();
				l += r;
			}

			value = encoding.GetString(buf);
		}
#else
		sealed class StringHelper
		{
			public StringHelper()
			{
				this.Encoding = new UTF8Encoding(false, true);
			}

			public const int BYTEBUFFERLEN = 256;
			public const int CHARBUFFERLEN = 128;

			Encoder m_encoder;
			Decoder m_decoder;

			byte[] m_byteBuffer;
			char[] m_charBuffer;

			public UTF8Encoding Encoding { get; private set; }
			public Encoder Encoder { get { if (m_encoder == null) m_encoder = this.Encoding.GetEncoder(); return m_encoder; } }
			public Decoder Decoder { get { if (m_decoder == null) m_decoder = this.Encoding.GetDecoder(); return m_decoder; } }

			public byte[] ByteBuffer { get { if (m_byteBuffer == null) m_byteBuffer = new byte[BYTEBUFFERLEN]; return m_byteBuffer; } }
			public char[] CharBuffer { get { if (m_charBuffer == null) m_charBuffer = new char[CHARBUFFERLEN]; return m_charBuffer; } }
		}

		[ThreadStatic]
		static StringHelper s_stringHelper;

		public unsafe static void WritePrimitive(Stream stream, string value)
		{
			if (value == null)
			{
				WritePrimitive(stream, (uint)0);
				return;
			}
			else if (value.Length == 0)
			{
				WritePrimitive(stream, (uint)1);
				return;
			}

			var helper = s_stringHelper;
			if (helper == null)
				s_stringHelper = helper = new StringHelper();

			var encoder = helper.Encoder;
			var buf = helper.ByteBuffer;

			int totalChars = value.Length;
			int totalBytes;

			fixed (char* ptr = value)
				totalBytes = encoder.GetByteCount(ptr, totalChars, true);

			WritePrimitive(stream, (uint)totalBytes + 1);
			WritePrimitive(stream, (uint)totalChars);

			int p = 0;
			bool completed = false;

			while (completed == false)
			{
				int charsConverted;
				int bytesConverted;

				fixed (char* src = value)
				fixed (byte* dst = buf)
				{
					encoder.Convert(src + p, totalChars - p, dst, buf.Length, true,
						out charsConverted, out bytesConverted, out completed);
				}

				stream.Write(buf, 0, bytesConverted);

				p += charsConverted;
			}
		}

		public static void ReadPrimitive(Stream stream, out string value)
		{
			uint totalBytes;
			ReadPrimitive(stream, out totalBytes);

			if (totalBytes == 0)
			{
				value = null;
				return;
			}
			else if (totalBytes == 1)
			{
				value = string.Empty;
				return;
			}

			totalBytes -= 1;

			uint totalChars;
			ReadPrimitive(stream, out totalChars);

			var helper = s_stringHelper;
			if (helper == null)
				s_stringHelper = helper = new StringHelper();

			var decoder = helper.Decoder;
			var buf = helper.ByteBuffer;
			char[] chars;
			if (totalChars <= StringHelper.CHARBUFFERLEN)
				chars = helper.CharBuffer;
			else
				chars = new char[totalChars];

			int streamBytesLeft = (int)totalBytes;

			int cp = 0;

			while (streamBytesLeft > 0)
			{
				int bytesInBuffer = stream.Read(buf, 0, Math.Min(buf.Length, streamBytesLeft));
				if (bytesInBuffer == 0)
					throw new EndOfStreamException();

				streamBytesLeft -= bytesInBuffer;
				bool flush = streamBytesLeft == 0 ? true : false;

				bool completed = false;

				int p = 0;

				while (completed == false)
				{
					int charsConverted;
					int bytesConverted;

					decoder.Convert(buf, p, bytesInBuffer - p,
						chars, cp, (int)totalChars - cp,
						flush,
						out bytesConverted, out charsConverted, out completed);

					p += bytesConverted;
					cp += charsConverted;
				}
			}

			value = new string(chars, 0, (int)totalChars);
		}
#endif

		public static void WritePrimitive(Stream stream, byte[] value)
		{
			if (value == null)
			{
				WritePrimitive(stream, (uint)0);
				return;
			}

			WritePrimitive(stream, (uint)value.Length + 1);

			stream.Write(value, 0, value.Length);
		}

		static readonly byte[] s_emptyByteArray = new byte[0];

		public static void ReadPrimitive(Stream stream, out byte[] value)
		{
			uint len;
			ReadPrimitive(stream, out len);

			if (len == 0)
			{
				value = null;
				return;
			}
			else if (len == 1)
			{
				value = s_emptyByteArray;
				return;
			}

			len -= 1;

			value = new byte[len];
			int l = 0;

			while (l < len)
			{
				int r = stream.Read(value, l, (int)len - l);
				if (r == 0)
					throw new EndOfStreamException();
				l += r;
			}
		}
	}
}
