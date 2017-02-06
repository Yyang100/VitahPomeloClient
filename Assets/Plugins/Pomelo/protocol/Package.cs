using System;

namespace Pomelo.DotNetClient
{
    public class Package
    {
        private PackageType type;
        private int length;
        private byte[] body;

        public Package(PackageType type, byte[] body)
        {
            this.type = type;
            this.length = body.Length;
            this.body = body;
        }

        public PackageType Type
        {
            get
            {
                return this.type;
            }
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }

        public byte[] Body
        {
            get
            {
                return this.body;
            }
        }
    }
}