using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class ImageLoaders : MonoBehaviour
{
    public Image test;
    public string customTextureFilename = "Image2.png";
    private UnityWebRequest uwr;

    public static string GetFileLocation(string relativePath)
    {
        return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
    }

    public void ChangeImage()
    {
        StartCoroutine(ChangeImageCo());
    }

    System.Collections.IEnumerator ChangeImageCo()
    {
        using (uwr = UnityWebRequestTexture.GetTexture(GetFileLocation(customTextureFilename)))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else 
            {
                //test.sprite = DownloadHandlerTexture.GetContent(uwr); 
            }
        }
    }
}






