using Pomelo.DotNetClient;
using SimpleJson;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PomeloManager : Singleton<PomeloManager>
{
    private PomeloClient pclient = new PomeloClient();

    public PomeloClient Pclient
    {
        get
        {
            return this.pclient;
        }
    }

    public void OnInit()
    {
        this.Pclient.NetWorkStateChangedEvent += (state) =>
        {
            if (state == NetWorkState.DISCONNECTED)
            {
                this.OnDisconnect();
            }
            else if (state == NetWorkState.KICK)
            {
                this.OnKick();
            }
            else if (state == NetWorkState.ERROR)
            {
                this.OnError();
            }
            else if (state == NetWorkState.TIMEOUT)
            {
                this.OnTimeout();
            }
        };
    }

    public void Connect(Action<ErrorCode, JsonObject> connectCallback)
    {
        if (this.Pclient.InitClient(ServerHost.Ip, ServerHost.Port) == false)
        {
            connectCallback(ErrorCode.FAIL, null);
            return;
        }

        this.Pclient.Connect((data) =>
            {
                connectCallback(ErrorCode.OK, data);
            });
    }

    public void Request(string route, JsonObject msg, Action<RequestData> onSuccess, Action onError)
    {
        this.Pclient.Request(
            route,
            msg,
            (statusCode, jsonObject) =>
            {
                if (statusCode == StatusCode.SUCCESS)
                {
                    RequestData data = null;
                    if (jsonObject != null)
                    {
                        Debug.Log(route + "\n" + jsonObject.ToString());
                        data = new RequestData(
                            JsonUtil.GetInt32(jsonObject, "code"),
                            JsonUtil.GetString(jsonObject, "msg"),
                            JsonUtil.GetJsonObject(jsonObject, "result"));
                        Loom.DispatchToMainThread(
                            () =>
                            {
                                onSuccess(data);
                            });
                    }
                }
                else
                {
                    onError();
                }
            });
    }

    public void Notify(string route, JsonObject msg)
    {
    }

    public void OnDisconnect()
    {
        Debug.Log("OnDisconnect");
        Loom.DispatchToMainThread(() => SceneManager.LoadScene("Login"));
    }

    public void OnKick()
    {
        Debug.Log("OnKick");
    }

    public void OnError()
    {
        Debug.Log("OnError");
    }

    public void OnTimeout()
    {
        Debug.Log("OnTimeout");
    }
}
