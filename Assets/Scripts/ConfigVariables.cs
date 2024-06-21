using System.IO;
using UnityEngine;

public static class ConfigVariables
{
    static ConfigVariables()
    {
        Config config = new Config();
        string path = Application.dataPath + "\\config.txt";

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                config.SERVER_PORT = 8090;
                config.SERVER_IP_ADDRESS = "178.150.247.139";
                config.REST_URI_ADDRESS = "http://178.150.247.139:8080/catan-api";
                sw.Write(JsonUtility.ToJson(config));
            }
        }
        else
        {
            using (StreamReader sr = new StreamReader(path))
            {
                config = (Config) JsonUtility.FromJson(sr.ReadToEnd(), typeof(Config));
            }
        }

        REST_URI_ADDRESS = config.REST_URI_ADDRESS;
        SERVER_IP_ADDRESS = config.SERVER_IP_ADDRESS;
        SERVER_PORT = config.SERVER_PORT;
    }

    public static string REST_URI_ADDRESS = "http://127.0.0.1:8080/catan-api";

    public static string SERVER_IP_ADDRESS = "127.0.0.1";

    public static short SERVER_PORT = 8090;

    public static string TOKEN;

    public static int Timeout = 5;
}
