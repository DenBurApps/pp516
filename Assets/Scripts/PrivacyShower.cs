using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyShower : MonoBehaviour
{
    [SerializeField] private UniWebView _uni;

    private void Start()
    {
        OpenPrivacy();
    }

    public void OpenPrivacy()
    {
        string url = PlayerPrefs.GetString("Link");
        _uni.Load(url);
        _uni.Show();
    }
}