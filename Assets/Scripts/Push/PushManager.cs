using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;

public class PushManager : Singleton<PushManager>
{
    public void Init()
    {
        this.Register(ProtocolEvent.ON_ROLE_CHANGE, RolePush.OnRole);
    }

    private void Register(string eventName, Action<JsonObject> action)
    {
        PomeloManager.Instance.Pclient.On(eventName, action);
    }
}