using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

/// <summary>
/// Inlcude all UI event functions
/// </summary>
public class UIEvents : MonoBehaviour
{
    public TextMeshProUGUI console;
    public Texture2D texture;
    public Image collectImageHolder;
    string imagePath = "";

    void OnEnable()
    {
        //NativeToolkit.OnScreenshotSaved += ScreenshotSaved;
        //NativeToolkit.OnImageSaved += ImageSaved;
        NativeToolkit.OnImagePicked += ImagePicked;
        NativeToolkit.OnCameraShotComplete += CameraShotComplete;

      
    }

    void OnDisable()
    {
        //NativeToolkit.OnScreenshotSaved -= ScreenshotSaved;
        //NativeToolkit.OnImageSaved -= ImageSaved;
        NativeToolkit.OnImagePicked -= ImagePicked;
        NativeToolkit.OnCameraShotComplete -= CameraShotComplete;
    }

    public void testFunction()
    {
        string path = "C:\\Users\\RuniJiang\\Downloads\\debug.png";
        console.text += "\nImage picked at: " + path;
        byte[] byteArray = File.ReadAllBytes(path);
        Debug.Log(Application.persistentDataPath);
        Texture2D texture = new Texture2D(8, 8);
        
        texture.LoadImage(byteArray);

        Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        collectImageHolder.sprite = s;

        string writePath = Application.persistentDataPath + "/test1.png";
        File.WriteAllBytes(writePath, byteArray);


    }

    //=============================================================================
    // Button handlers
    //=============================================================================

    public void OnSaveScreenshotPress()
    {
        NativeToolkit.SaveScreenshot("MyScreenshot", "MyScreenshotFolder", "jpeg");
    }

    public void OnSaveImagePress()
    {
        NativeToolkit.SaveImage(texture, "MyImage", "png");
    }

    public void OnPickImagePress()
    {
        NativeToolkit.PickImage();
    }
    public void OnCameraPress()
    {
        NativeToolkit.TakeCameraShot();
    }



    //=============================================================================
    // Callbacks
    //=============================================================================

    void ScreenshotSaved(string path)
    {
        console.text += "\n" + "Screenshot saved to: " + path;
    }

    void ImageSaved(string path)
    {
        console.text += "\n" + texture.name + " saved to: " + path;
    }

    void ImagePicked(Texture2D img, string path)
    {

        imagePath = path;
        console.text += "\nImage picked at: " + imagePath;
       // byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + "/" + imagePath);
        byte[] byteArray = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(8, 8);
        texture.LoadImage(byteArray);
        Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        collectImageHolder.sprite = s;
        Destroy(img);
    }

    void CameraShotComplete(Texture2D img, string path)
    {
        imagePath = path;
        //background.ImageConversion.LoadImage(path);
        console.text += "\nCamera shot saved to: " + imagePath;
        Destroy(img);
    }


    /// <summary>
    /// Try to access the camera or camera roll
    /// </summary>
    public void AccessCamera()
    {
        Debug.Log("Accessing Camera");
    }

    /// <summary>
    /// Allow user to take notes and save locally
    /// </summary>
    public void NoteTaking()
    { 
        Debug.Log("Note Taking");
    }

    /// <summary>
    /// Access user voice input and save locally
    /// </summary>
    public void MicRecord()
    {
        Debug.Log("MicRecord");
    }

}
