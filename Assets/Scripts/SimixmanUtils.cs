using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class SimixmanUtils
{
    public static async Task<RestResponseData> SendRequest<T>(string uri, T content, HttpMethod httpMethod, bool isBody = true, bool isAuthorized = false) where T : RestDTOClass
    {
        using HttpClient httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(StaticVariables.Timeout);
        using HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
        if (isAuthorized)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", StaticVariables.TOKEN);
        }
        httpRequestMessage.RequestUri = new Uri(uri);
        httpRequestMessage.Method = httpMethod;

        if (isBody)
        {
            string s = JsonUtility.ToJson(content);
            httpRequestMessage.Content = new StringContent(s, Encoding.UTF8, "application/json");
        }

        RestResponseData restResponse = new RestResponseData();

        try
        {
            using HttpResponseMessage httpResponceMessage = await httpClient.SendAsync(httpRequestMessage);
            restResponse.resultStatusCode = httpResponceMessage.StatusCode;
            restResponse.jsonResponseBody = await httpResponceMessage.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            restResponse.resultStatusCode = HttpStatusCode.BadRequest;
            restResponse.jsonResponseBody = e.Message;
        }

        return restResponse;
    }

    public static bool IsConnected(this Socket s)
    {
        if (s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
            return false;
        return true;
    }

    public static async Task<bool> SendToServer<T>(this Socket socket, T content) where T : SocketDTOClass
    {
        try
        {
            string json = JsonUtility.ToJson(content) + "/nq";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(sendBuffer, SocketFlags.None);
            Debug.Log("Send To Server: Send data to server with type " + content.GetType().ToString());
            return true;
        }
        catch (Exception)
        {
            Debug.Log("Send To Server: Failed to send data");
            return false;
        }
    }

    public static string FixArrayJson(string json)
    {
        return "{\"Items\":" + json + "}";
    }

    public static Vector2 DecimalSumOfTwoVectors2D(Vector2 firstVector, Vector2 secondVector)
    {
        decimal x = (decimal)firstVector.x + (decimal)secondVector.x;
        decimal y = (decimal)firstVector.y + (decimal)secondVector.y;
        return new Vector2((float)x, (float)y);
    }

    public static Dictionary<Resource, int> ResourceListToResourceDictionary(List<string> resources)
    {
        Dictionary<Resource, int> resourcesToAmount = new();
        foreach (string s in resources)
        {
            Resource resource = (Resource)Enum.Parse(typeof(Resource), s);
            if (resourcesToAmount.ContainsKey(resource))
            {
                resourcesToAmount[resource]++;
            }
            else
            {
                resourcesToAmount.Add(resource, 1);
            }
        }

        return resourcesToAmount;
    }
    
    public static List<Resource> ResourceDictionaryToResourceList(Dictionary<Resource, int> resourcesToAmount)
    {
        List<Resource> resources = new();
        foreach(var resourceToAmount in resourcesToAmount)
        {
            for(int i = 0; i < resourceToAmount.Value; i++)
            {
                resources.Add(resourceToAmount.Key);
            }
        }

        return resources;
    }
}

