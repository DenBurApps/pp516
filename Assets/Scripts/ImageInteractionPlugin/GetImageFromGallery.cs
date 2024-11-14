using System;
using UnityEngine;
using UnityEngine.UI;

public static class GetImageFromGallery
{
    private static string _path = "";

    public static bool SetImage(string path, Image image)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path, -1);
        
        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
            image.sprite = sprite;
            return true;
        }

        return false;
    }

    public static void PickImage(Action<string> ImagePicked)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    _path = "";
                    return;
                }
                _path = path;
            }
            else
            {
                _path = "";
            }
            ImagePicked?.Invoke(_path);
        });
    }
}
