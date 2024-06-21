using System;
using System.IO;
using UnityEngine;

public static class SimixmanLogger
{
    private static string folderPath;
    private static string filePath;

    static SimixmanLogger()
    {
        folderPath = Application.dataPath + "/Logs";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        filePath = folderPath + $"/log-{DateTime.Now}".Replace(" ", "_").Replace(":", "_").Replace(".","_") + ".txt";
        using StreamWriter sw = File.CreateText(filePath);
    }

    public static void Log(object message)
    {
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"[{DateTime.Now}] " + message.ToString());
        }
        Debug.Log(message);
    }

    public static void LogError(object message)
    {
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"[{DateTime.Now}] ERROR: " + message.ToString());
        }
        Debug.Log(message);
    }
}
