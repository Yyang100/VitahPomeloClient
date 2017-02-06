using SimpleJson;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Pomelo.DotNetClient
{
    public class EventManager : IDisposable
    {
        // 10 second
        private const int TimeoutSec = 10;

        // 1 second
        private const int TimeoutInterval = 1000;

        private Dictionary<uint, PackageCallBack> callBackMap;
        private Dictionary<string, List<Action<JsonObject>>> eventMap;

        // timeout event
        private Timer timer;

        public EventManager()
        {
            this.callBackMap = new Dictionary<uint, PackageCallBack>();
            this.eventMap = new Dictionary<string, List<Action<JsonObject>>>();
            this.StartTimeOut();
        }

        // Adds callback to callBackMap by id.
        public void AddCallBack(uint id, Action<StatusCode, JsonObject> callback)
        {
            if (id > 0 && callback != null)
            {
                PackageCallBack packageCallBack = new PackageCallBack();
                packageCallBack.CallBack = callback;
                packageCallBack.SendTime = DateTime.Now;
                this.callBackMap.Add(id, packageCallBack);
            }
        }

        // Invoke the callback when the server return messge .
        public void InvokeCallBack(uint id, StatusCode code, JsonObject data)
        {
            if (!this.callBackMap.ContainsKey(id))
            {
                return;
            }

            PackageCallBack packageCallback = this.callBackMap[id];
            this.callBackMap.Remove(id);
            packageCallback.CallBack(code, data);
        }

        // Adds the event to eventMap by name.
        public void AddOnEvent(string eventName, Action<JsonObject> callback)
        {
            List<Action<JsonObject>> list = null;
            if (this.eventMap.TryGetValue(eventName, out list))
            {
                list.Add(callback);
            }
            else
            {
                list = new List<Action<JsonObject>>();
                list.Add(callback);
                this.eventMap.Add(eventName, list);
            }
        }

        // If the event exists,invoke the event when server return messge.
        public void InvokeOnEvent(string route, JsonObject msg)
        {
            if (!this.eventMap.ContainsKey(route))
            {
                return;
            }

            List<Action<JsonObject>> list = this.eventMap[route];
            foreach (Action<JsonObject> action in list)
            {
                action.Invoke(msg);
            }
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected void Dispose(bool disposing)
        {
            this.callBackMap.Clear();
            this.eventMap.Clear();
        }

        private void StartTimeOut()
        {
            this.timer = new Timer();
            this.timer.Interval = TimeoutInterval;
            this.timer.Elapsed += new ElapsedEventHandler(this.ProcessTimeOut);
            this.timer.Enabled = true;
        }

        private void ProcessTimeOut(object source, ElapsedEventArgs e)
        {
            if (this.callBackMap.Count <= 0)
            {
                return;
            }

            List<uint> listKey = new List<uint>(this.callBackMap.Keys);
            for (int i = 0; i < listKey.Count; i++)
            {
                uint reqId = listKey[i];
                PackageCallBack packageCallback = this.callBackMap[reqId];
                if (DateTime.Now.Subtract(packageCallback.SendTime).TotalSeconds > TimeoutSec)
                {
                    this.InvokeCallBack(reqId, StatusCode.TIMEOUT, null);
                }
            }
        }
    }
}