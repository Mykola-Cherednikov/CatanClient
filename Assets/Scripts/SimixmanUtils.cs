using Assets.Scripts.DTO;
using Assets.Scripts.RestDTO;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts
{
    public static class SimixmanUtils
    {
        public static async Task<RestResult> SendRequest<T>(string uri, T content, HttpMethod method, bool isBody = true, bool isAuthorized = false) where T : RestDTOClass
        {
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(StaticVariables.Timeout);
            using HttpRequestMessage httpRequest = new HttpRequestMessage();
            if (isAuthorized)
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", StaticVariables.TOKEN);
            }
            httpRequest.RequestUri = new Uri(uri);
            httpRequest.Method = method;

            if (isBody)
            {
                string s = JsonUtility.ToJson(content);
                httpRequest.Content = new StringContent(s, Encoding.UTF8, "application/json");
            }

            RestResult result = new RestResult();

            try
            {
                using HttpResponseMessage httpResponce = await client.SendAsync(httpRequest);

                result.resultStatusCode = httpResponce.StatusCode;
                result.resultData = await httpResponce.Content.ReadAsStringAsync();
            } catch (Exception e)
            {
                result.resultStatusCode = HttpStatusCode.BadRequest;
                result.resultData = e.Message;
            }

            return result;
        }

        public static bool IsConnected(this Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = s.Available == 0;
            if (part1 && part2)
                return false;
            return true;
        }

        public static async Task<bool> SendToServer<T>(this Socket socket, T data) where T : SocketDTOClass
        {
            try
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes(JsonUtility.ToJson(data) + "/nq"), SocketFlags.None);
                Debug.Log("Send To Server: Send data to server with type " + data.GetType().ToString());
                return true;
            } catch (Exception)
            {
                Debug.Log("Send To Server: Failed to send data");
                return false;
            }
        }

        public static string FixArrayJson(string json)
        {
            return "{\"Items\":" + json + "}";
        }
    }
}
