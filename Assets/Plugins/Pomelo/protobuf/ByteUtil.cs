using System;
using System.Text;

namespace Pomelo.Protobuf
{
    public class ByteUtil
    {
        public static int StringByteLength(string msg)
        {
            return System.Text.Encoding.UTF8.GetBytes(msg).Length;
        }

        public static ushort ReadShort(int offset, byte[] bytes)
        {
            ushort result = 0;
            result += (ushort)(bytes[offset] << 8);
            result += (ushort)(bytes[offset + 1]);
            return result;
        }

        public static void WriteShort(int offset, ushort value, byte[] bytes)
        {
            bytes[offset] = (byte)(value >> 8 & 0xff);
            bytes[offset + 1] = (byte)(value & 0xff);
        }

        public static void WriteInt(int offset, uint value, byte[] bytes)
        {
            bytes[offset] = (byte)(value >> 24 & 0xff);
            bytes[offset + 1] = (byte)(value >> 16 & 0xff);
            bytes[offset + 2] = (byte)(value >> 8 & 0xff);
            bytes[offset + 3] = (byte)(value & 0xff);
        }

        public static void WriteRawLittleEndian64(byte[] buffer, int offset, ulong value)
        {
            buffer[offset++] = ((byte)value);
            buffer[offset++] = ((byte)(value >> 8));
            buffer[offset++] = ((byte)(value >> 16));
            buffer[offset++] = ((byte)(value >> 24));
            buffer[offset++] = ((byte)(value >> 32));
            buffer[offset++] = ((byte)(value >> 40));
            buffer[offset++] = ((byte)(value >> 48));
            buffer[offset++] = ((byte)(value >> 56));
        }

        // Write bytes to buffer.
        public static int WriteBytes(byte[] source, int offset, byte[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[offset] = source[i];
                offset++;
            }

            return offset;
        }

        // Encode string.
        public static void WriteString(byte[] buffer, ref int offset, object value)
        {
            int le = Encoding.UTF8.GetByteCount(value.ToString());
            offset = ByteUtil.WriteBytes(Encoder.EncodeUInt32((uint)le), offset, buffer);
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            ByteUtil.WriteBytes(bytes, offset, buffer);
            offset += le;
        }

        // Encode Int32
        public static void WriteInt32(byte[] buffer, ref int offset, object value)
        {
            offset = ByteUtil.WriteBytes(Encoder.EncodeSInt32(value.ToString()), offset, buffer);
        }

        // Encode float.
        public static void WriteFloat(byte[] buffer, ref int offset, object value)
        {
            ByteUtil.WriteBytes(Encoder.EncodeFloat(float.Parse(value.ToString())), offset, buffer);
            offset += 4;
        }

        // Encode double.
        public static void WriteDouble(byte[] buffer, ref int offset, object value)
        {
            ByteUtil.WriteRawLittleEndian64(buffer, offset, (ulong)BitConverter.DoubleToInt64Bits(double.Parse(value.ToString())));
            offset += 8;
        }

        // Encode UInt32.
        public static void WriteUInt32(byte[] buffer, ref int offset, object value)
        {
            offset = ByteUtil.WriteBytes(Encoder.EncodeUInt32(value.ToString()), offset, buffer);
        }
    }
}