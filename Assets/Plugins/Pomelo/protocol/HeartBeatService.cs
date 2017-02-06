using System;
using System.Timers;

namespace Pomelo.DotNetClient
{
    public class HeartBeatService
    {
        int interval;
        public int timeout;
        Timer timer;
        DateTime lastTime;
        Protocol protocol;

        public HeartBeatService(int interval, Protocol protocol)
        {
            this.interval = interval * 1000;
            this.protocol = protocol;
        }

        public void ResetTimeout()
        {
            this.timeout = 0;
            this.lastTime = DateTime.Now;
        }

        public void SendHeartBeat(object source, ElapsedEventArgs e)
        {
            TimeSpan span = DateTime.Now - this.lastTime;
            this.timeout = (int)span.TotalMilliseconds;

            // check timeout
            if (this.timeout > this.interval * 2)
            {
                this.protocol.GetPomeloClient().Disconnect();
                this.Stop();
                return;
            }

            // Send heart beat
            this.protocol.Send(PackageType.PKG_HEARTBEAT);
        }

        public void Start()
        {
            if (this.interval < 1000)
            {
                return;
            }

            // start hearbeat
            this.timer = new Timer();
            this.timer.Interval = this.interval;
            this.timer.Elapsed += new ElapsedEventHandler(this.SendHeartBeat);
            this.timer.Enabled = true;

            // set timeout
            this.timeout = 0;
            this.lastTime = DateTime.Now;
        }

        public void Stop()
        {
            if (this.timer != null)
            {
                this.timer.Enabled = false;
                this.timer.Dispose();
            }
        }
    }
}