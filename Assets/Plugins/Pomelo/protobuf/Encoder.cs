using System;
using System.Collections;
using System.Collections.Generic;

namespace Pomelo.Protobuf
{
    public class Encoder
    {
        public static byte[] EncodeUInt32(string n)
        {
            return EncodeUInt32(Convert.ToUInt32(n));
        }

        public static byte[] EncodeUInt32(uint n)
        {
            List<byte> byteList = new List<byte>();
            do
            {
                uint tmp = n % 128;
                uint next = n >> 7;
                if (next != 0)
                {
                    tmp = tmp + 128;
                }

                byteList.Add(Convert.ToByte(tmp));
                n = next;
            }
            while (n != 0);

            return byteList.ToArray();
        }

        public static byte[] EncodeSInt32(string n)
        {
            return EncodeSInt32(Convert.ToInt32(n));
        }

        public static byte[] EncodeSInt32(int n)
        {
            uint num = (uint)(n < 0 ? (Math.Abs(n) * 2 - 1) : n * 2);
            return EncodeUInt32(num);
        }

        public static byte[] EncodeFloat(float n)
        {
            byte[] bytes = BitConverter.GetBytes(n);
            if (!BitConverter.IsLittleEndian)
            {
                Util.Reverse(bytes);
            }

            return bytes;
        }
    }
}