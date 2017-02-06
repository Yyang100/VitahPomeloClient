using System;
using System.Collections;
using System.Collections.Generic;

namespace Pomelo.Protobuf
{
    public class Util
    {
        // Simple type
        private string[] types;
        private Dictionary<string, int> typeMap;

        public Util()
        {
            this.InitTypeMap();
            this.types = new string[] { "uInt32", "sInt32", "int32", "uInt64", "sInt64", "float", "double" };
        }

        // Reverses the order of bytes in the array
        public static void Reverse(byte[] bytes)
        {
            byte temp;
            for (int first = 0, last = bytes.Length - 1; first < last; first++, last--)
            {
                temp = bytes[first];
                bytes[first] = bytes[last];
                bytes[last] = temp;
            }
        }

        // Check out the given type. If it is simple, return ture.
        public bool IsSimpleType(string type)
        {
            int length = this.types.Length;
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                if (type == this.types[i])
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        // Check out the given type. If the type exist in typeMap, return true.
        public int ContainType(string type)
        {
            int value, returnInt = 2;
            if (this.typeMap.TryGetValue(type, out value))
            {
                returnInt = value;
            }

            return returnInt;
        }

        // Init the typeMap
        private void InitTypeMap()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("uInt32", 0);
            dic.Add("sInt32", 0);
            dic.Add("int32", 0);
            dic.Add("double", 1);
            dic.Add("string", 2);
            dic.Add("float", 5);
            dic.Add("message", 2);

            this.typeMap = dic;
        }
    }
}