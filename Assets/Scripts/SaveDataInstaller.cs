using Flagsmith;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;

public class SaveDataInstaller : MonoBehaviour
{
    [SerializeField] private bool _fromTheBeginning;
    [SerializeField] private string _key;
    [SerializeField] private ConfigData _allConfigData;
    private bool _showTerms = true;
    private bool usePrivacy;
    private string link;
    private static FlagsmithClient _flagsmithClient;

    private void Start()
    {
#if UNITY_IOS
        if (!PlayerPrefs.HasKey("Onboarding"))
        {
        Device.RequestStoreReview();
        }
#endif
        _flagsmithClient = new(_key);
        StartLoading();
    }

    private void StartLoading()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");

        Debug.Log("Google result: " + HtmlText);

        if (HtmlText != "")
        {
            LoadFirebaseConfig();
        }

        else
        {
            Debug.Log("No internet");
            LoadScene();
        }
    }

    public void LoadFirebaseConfig()
    {
        _ = StartAsync();
    }

    async Task StartAsync()
    {
        var flags = await _flagsmithClient.GetEnvironmentFlags();
        if(flags == null)
        {
            Debug.Log("flags null");
            LoadScene();
            return;
        }
        string values = await flags.GetFeatureValue("config");
        if(values == null || values == "")
        {
            Debug.Log("values null");
            LoadScene();
            return;
        }
        Debug.Log("Loaded");
        ProcessJsonResponse(values);
    }

    private void ProcessJsonResponse(string jsonResponse)
    {
        ConfigData config = JsonConvert.DeserializeObject<ConfigData>(jsonResponse);
        _allConfigData.link = config.link;
        _allConfigData.usePrivacy = config.usePrivacy;

        Debug.Log("link's value from Config: " + _allConfigData.link);
        Debug.Log("useprivacy's value from Config: " + _allConfigData.usePrivacy);

        _showTerms = _allConfigData.usePrivacy;
        PlayerPrefs.SetString("Link", _allConfigData.link);
        LoadScene();

    }

    private void LoadScene()
    {
        if (_showTerms)
        {
            if (PlayerPrefs.HasKey("Onboarding"))
            {
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                SceneManager.LoadScene("OnboardingScene");
            }
        }
        else
        {
            SceneManager.LoadScene("TestScene");
        }
        
    }

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }

}

[Serializable]
public class ConfigData
{
    public string link;
    public bool usePrivacy;
}