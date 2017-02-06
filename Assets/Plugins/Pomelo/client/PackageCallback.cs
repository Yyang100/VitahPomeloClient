using SimpleJson;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Pomelo.DotNetClient
{
    public class PackageCallBack
    {
        public Action<StatusCode, JsonObject> CallBack
        {
            get;
            set;
        }

        public DateTime SendTime
        {
            get;
            set;
        }
    }
}