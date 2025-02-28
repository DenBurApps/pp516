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
        //Подгружаем сохранённую ссылку в вебвью в зависимости от вашей системы сохранений
        var link = LinkSaver.Link;
        _uni.Load(link);
        _uni.Show();
    }
}