using Pomelo.DotNetClient;
using SimpleJson;
using System;

public class BaseRequest
{
    public Action<RequestData> OnSuccess;
    public Action<RequestData> OnError;
    public Action OnFail;

    protected void Request(string route, JsonObject msg)
    {
        PomeloManager.Instance.Request(
            route,
            msg,
            (data) =>
            {
                if (data.Code == 200)
                {
                    if (this.OnSuccess != null)
                    {
                        this.OnSuccess(data);
                    }
                }
                else
                {
                    if (this.OnError != null)
                    {
                        this.OnError(data);
                    }
                }
            },
            () =>
            {
                if (this.OnFail != null)
                {
                    this.OnFail();
                }
            });
    }

    protected void Notify(string route, JsonObject msg)
    {
        PomeloManager.Instance.Notify(route, msg);
    }
}
