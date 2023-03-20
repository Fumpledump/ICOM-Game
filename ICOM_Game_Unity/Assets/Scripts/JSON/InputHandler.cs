using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;



public class InputHandler : MonoBehaviour
{
    public GameObject slotPrefab;
    public Sprite defaultImage;

    [SerializeField] TMP_InputField titleInput;
    [SerializeField] string curImageFilePath;
    [SerializeField] TMP_InputField noteInput;
    [SerializeField] string filename;

    List<InputEntry> entries = new List<InputEntry>();
    List<GameObject> collectionSlots = new List<GameObject>();
    List<Sprite> collectionSprite = new List<Sprite>();
    private int curCollectionIndex = -1;

    // Image Part
    public GameObject collectionPage;
    public GameObject inventoryPage;
    public GameObject unsavedConfirmationPage;
    public GameObject deleteConfirmPage;
    public GameObject grid;
    public TextMeshProUGUI console;
    public Texture2D texture;
    public Image collectImageHolder;
    [SerializeField] string imagePath;

    private void Start()
    {
        entries = FileHandler.ReadListFromJSON<InputEntry>(filename);
        for (int i = 0; i < entries.Count; i++)
        {
            InputEntry entry = entries[i];
            collectionSlots.Add(Instantiate(slotPrefab));
            collectionSlots[i].transform.parent = grid.transform;
            Button collectionBtn = collectionSlots[i].GetComponent<Button>();
            collectionBtn.onClick.RemoveAllListeners();
            collectionBtn.onClick.AddListener(() => {
                collectionPage.SetActive(true);
                inventoryPage.SetActive(false);
                LoadName(entry);
                curCollectionIndex = entry.Index;
                Debug.Log(entry.Index);
            });
            Image collectionImage = collectionSlots[i].GetComponent<Image>();
            LoadRawImage(entry, collectionImage, i);
            LoadSprite(collectionImage, i);
        }
    }

    public void UpdateInventory(InputEntry entry)
    {
        collectionSlots.Add(Instantiate(slotPrefab));
        collectionSlots[collectionSlots.Count - 1].transform.parent = grid.transform;
        Button collectionBtn = collectionSlots[collectionSlots.Count - 1].GetComponent<Button>();
        collectionBtn.onClick.RemoveAllListeners();
        collectionBtn.onClick.AddListener(() => {
            collectionPage.SetActive(true);
            inventoryPage.SetActive(false);
            curCollectionIndex = entry.Index;
            LoadName(entry);
        });
        Image collectionImage = collectionSlots[collectionSlots.Count - 1].GetComponent<Image>();
        LoadRawImage(entry, collectionImage, entry.Index);
        LoadSprite(collectionImage, entry.Index);
    }

    public void CreateNewCollection()
    {
        inventoryPage.SetActive(false);
        collectionPage.SetActive(true);

        curCollectionIndex= entries.Count;

        collectImageHolder.sprite = defaultImage;
        titleInput.text = "";
        curImageFilePath = "";
        noteInput.text = "";
    }

    public void AddNameToList()     
    {
        //entries.Add(new InputEntry(entries.Count, titleInput.text, imageFilePath, string recordingFilePath, noteInput));

        if (curCollectionIndex < entries.Count && curCollectionIndex >= 0)
        {
            entries[curCollectionIndex].Title = titleInput.text;
            entries[curCollectionIndex].ImageFilePath= curImageFilePath;
            entries[curCollectionIndex].Notes = noteInput.text;
            LoadRawImage(entries[curCollectionIndex], collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
            LoadSprite(collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
        }
        else
        {
            entries.Add(new InputEntry(entries.Count, titleInput.text, curImageFilePath, noteInput.text));
            UpdateInventory(entries[entries.Count - 1]);
        }
     
        //nameInput.text = "";

        FileHandler.SaveToJSON<InputEntry>(entries, filename);
    }

    public void Delete()
    {

        // Back to inventory Page
        inventoryPage.SetActive(true);
        collectionPage.SetActive(false);

        if (curCollectionIndex >= entries.Count)
        {
            return;
        }


        // Remove the entries and collection in file system
        Debug.Log(curCollectionIndex);
        entries.RemoveAt(curCollectionIndex);
        collectionSprite.RemoveAt(curCollectionIndex);
        Debug.Log("Delete the entries in file system");
        for (int i = curCollectionIndex; i < entries.Count; i++)
        {
            Debug.Log("Update the index " + entries[i].Index + " to index " + i);
            entries[i].Index = i;
        }
        FileHandler.SaveToJSON<InputEntry>(entries, filename);
        Debug.Log("Current collection Index: " + curCollectionIndex);
        Destroy(collectionSlots[curCollectionIndex]);
        collectionSlots.RemoveAt(curCollectionIndex);
        Debug.Log("Deleted");
    }

    public void CheckCloseUnsaved()
    {
        Debug.Log("check unsave0d " + curCollectionIndex);
        if (curCollectionIndex == entries.Count)
        {
            Debug.Log("New things " + curCollectionIndex);
            if (titleInput.text != "" || curImageFilePath != "" || noteInput.text != "")
            {
                unsavedConfirmationPage.SetActive(true);
            }
            else
            {
                collectionPage.SetActive(false);
                inventoryPage.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Old things " + curCollectionIndex);
            if (titleInput.text != entries[curCollectionIndex].Title ||
                curImageFilePath != entries[curCollectionIndex].ImageFilePath ||
                noteInput.text != entries[curCollectionIndex].Notes)
            {
                unsavedConfirmationPage.SetActive(true);
            }
            else
            {
                collectionPage.SetActive(false);
                inventoryPage.SetActive(true);
            }
        }
    }

    public void LoadName(InputEntry collection)
    {
        curCollectionIndex = collection.Index;
        curImageFilePath = entries[curCollectionIndex].ImageFilePath;
        titleInput.text = collection.Title;
        //LoadRawImage(collection, collectImageHolder);
        LoadSprite(collectImageHolder, collection.Index);
        noteInput.text = collection.Notes;
        Debug.Log("LoadName " + curCollectionIndex);
    }

    public void LoadSprite(Image holder, int index)
    {
        holder.sprite = collectionSprite[index];
    }
    public void LoadRawImage(InputEntry collection, Image holder, int index)
    {
        //Debug.Log(entries[entries.Count - 1].ImageFilePath);
        //  && File.Exists(collection.ImageFilePath)
        curImageFilePath = collection.ImageFilePath;
        if (collection.ImageFilePath != "")
        {
            Sprite s = IMG2Sprite.LoadNewSprite(collection.ImageFilePath);
            //collectionSprite.Add(s);
            collectionSprite.Insert(index, s);

            //byte[] byteArray = File.ReadAllBytes(collection.ImageFilePath);

            //Texture2D texture = new Texture2D(8, 8);
            //texture.LoadImage(byteArray);
            //Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
            
            //holder.sprite = s;
        }
        else 
        {
            collectionSprite.Insert(index, defaultImage);
            //collectionSprite.Add(defaultImage);
            //holder.sprite = defaultImage;
        }
    }

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

        string writePath = Application.persistentDataPath + $"/test_{entries.Count - 1}.png";
        curImageFilePath = writePath;
        File.WriteAllBytes(writePath, byteArray);

        Destroy(img);
    }


    public void testFunction()
    {
        string path = "C:\\Users\\RuniJiang\\Downloads\\catPawClear.png";
        console.text += "\nImage picked at: " + path;

        byte[] byteArray = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(8, 8);
        texture.LoadImage(byteArray);
        Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        collectImageHolder.sprite = s;

        string writePath = Application.persistentDataPath + $"/test_{entries.Count - 1}.png";
        curImageFilePath = writePath;
        File.WriteAllBytes(writePath, byteArray);
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