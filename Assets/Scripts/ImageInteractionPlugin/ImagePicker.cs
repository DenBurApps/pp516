using System;
using UnityEngine;
using UnityEngine.UI;

public class ImagePicker : MonoBehaviour
{
    [SerializeField] private PreferedLocale _preferedExtension = PreferedLocale.Width;
    [SerializeField] private Image _image;


    public string CurrentPath;

    private Texture _imageTexture;

    private Vector2 _standartSize;

    public Image Image => _image;

    public void OnDisable()
    {
        Destroy(_imageTexture);
    }

    public void OnDestroy()
    {
        Destroy(_imageTexture);
    }
    
    public void Init(string path)
    {
        var imgTransform = _image.GetComponent<RectTransform>();
        _standartSize = new Vector2(imgTransform.sizeDelta.x, imgTransform.sizeDelta.y); 
        //Debug.Log(_standartSize);
        
        if (path != "" && path != null)
        {
            //_image.gameObject.SetActive(true);

            Destroy(_imageTexture);
            GetImageFromGallery.SetImage(path, _image);
            _imageTexture = _image.mainTexture; 

            CurrentPath = path;
        }
        else
            Debug.Log("Path is not correct");
        SetNormalSize();
    }

    private void SetNormalSize()
    {
        Texture texture = _image.mainTexture;

        float differenceInImage;

        if (_preferedExtension == PreferedLocale.Width)
        {
            differenceInImage = _standartSize.x / texture.width;
        }
        else
        {
            differenceInImage = _standartSize.y / texture.height;

        }
        float normalWidth = texture.width * differenceInImage;
        float normalHeight = texture.height * differenceInImage;
        var imgTransform = _image.GetComponent<RectTransform>();

        imgTransform.sizeDelta = new Vector2(normalWidth, normalHeight);
    }
}

[Serializable]
public enum PreferedLocale
{
    Width,
    Height
}