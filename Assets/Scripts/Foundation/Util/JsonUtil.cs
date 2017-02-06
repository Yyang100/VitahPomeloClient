using SimpleJson;
using System;
using System.Collections.Generic;

public class JsonUtil
{
    public static int GetInt32(JsonObject json_object, string key, int defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToInt32(json_object[key]);
    }

    public static uint GetUInt32(JsonObject json_object, string key, uint defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToUInt32(json_object[key]);
    }

    public static float GetFloat(JsonObject json_object, string key, float defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToSingle(json_object[key]);
    }

    public static double GetDouble(JsonObject json_object, string key, double defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToDouble(json_object[key]);
    }

    public static string GetString(JsonObject json_object, string key, string defaultValue = "")
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToString(json_object[key]);
    }

    public static bool GetBool(JsonObject json_object, string key, bool defaultValue = false)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToBoolean(json_object[key]);
    }

    public static JsonObject GetJsonObject(JsonObject json_object, string key)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return null;
        }

        return (JsonObject)json_object[key];
    }

    public static JsonArray GetJsonArray(JsonObject json_object, string key)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return new JsonArray();
        }

        return json_object[key] as JsonArray;
    }

    public static List<object> GetJsonList(JsonObject json_object, string key)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return null;
        }

        return json_object[key] as List<object>;
    }

    public static long GetInt64(JsonObject json_object, string key, long defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToInt64(json_object[key]);
    }

    public static ulong GetUInt64(JsonObject json_object, string key, ulong defaultValue = 0)
    {
        if (json_object == null || json_object.ContainsKey(key) == false)
        {
            return defaultValue;
        }

        return Convert.ToUInt64(json_object[key]);
    }
}