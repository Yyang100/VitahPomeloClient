using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;

public class RolePush
{
    public static void OnRole(JsonObject jsonObject)
    {
        Debug.Log("role push : " + jsonObject); // todo
        DataPool.Instance.Role.Init(jsonObject);
        PushEventNotifyCenter.Instance.FireChangeEvent(ProtocolFeature.OnRole);
    }
}