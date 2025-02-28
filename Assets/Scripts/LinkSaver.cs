using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public static class LinkSaver
{
    private static string SavePath = Path.Combine(Application.persistentDataPath, "link");

    public static string Link = GetLink();
    
    public static void SaveLink(string link)
    {
        try
        {
            string json = JsonUtility.ToJson(link, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("link saved");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public static string GetLink()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                string link = JsonUtility.FromJson<string>(json);
                Debug.Log("link loaded");
                return link;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return string.Empty;
            }
        }
        
        return string.Empty;
    }
}