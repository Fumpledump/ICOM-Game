using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
using System.Text;



public class InputHandler : MonoBehaviour
{
    public GameObject slotPrefab;
    public Sprite defaultImage;

    // Mic variables
    public GameObject mic;
    private Recorder recorder;
    private AudioPlayer audioPlayer;

    [SerializeField] TMP_InputField titleInput;
    [SerializeField] string curImageFilePath;
    [SerializeField] string curRecordingFilePath;
    [SerializeField] TMP_InputField noteInput;
    [SerializeField] string filename;

    List<InputEntry> entries = new List<InputEntry>();
    List<GameObject> collectionSlots = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    List<Sprite> collectionSprite = new List<Sprite>();
    private int curCollectionIndex = -1;

    // Canvas Part
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
        recorder  = mic.GetComponent<Recorder>();
        audioPlayer = mic.GetComponent<AudioPlayer>();

        // Initialization
        // Read from json file
        entries = FileHandler.ReadListFromJSON<InputEntry>(filename);

        // Loop through each of the entries
        for (int i = 0; i < entries.Count; i++)
        {
            InputEntry entry = entries[i];
            // set up the grid
            // ----------- In the future, this part will be modified to the real game object ----- //
            collectionSlots.Add(Instantiate(slotPrefab));
            collectionSlots[i].transform.parent = grid.transform;

            Button collectionBtn = collectionSlots[i].GetComponent<Button>();
            collectionBtn.onClick.RemoveAllListeners();
            collectionBtn.onClick.AddListener(() => {
                OpenCollectionPage(entry);
                curCollectionIndex = entry.Index;
            });
            // grid image showing
            Image collectionImage = collectionSlots[i].GetComponent<Image>();
            LoadRawImage(entry, collectionImage, i);
            LoadSprite(collectionImage, i);
        }
    }

    /// <summary>
    /// Load the item/collection into the collection page
    /// </summary>
    /// <param name="collection">loaded collection</param>
    public void LoadInfoToCollectionPage(InputEntry collection)
    {
        curCollectionIndex = collection.Index;
        curImageFilePath = collection.ImageFilePath;
        curRecordingFilePath = collection.RecordingFilePath;
        titleInput.text = collection.Title;
        noteInput.text = collection.Notes;

        LoadSprite(collectImageHolder, collection.Index);
        Debug.Log("LoadName " + curCollectionIndex);

        //if (collection.RecordingFilePath != "")
        //{
        //    recorder.fileName = collection.RecordingFilePath;
        //    audioPlayer.LoadAudio();
        //}
    }

    /// <summary>
    /// Update Inventory when new item is added
    /// This will be rework when there is real inventory instead of grid
    /// </summary>
    /// <param name="entry">added item</param>
    public void UpdateInventory(InputEntry entry)
    {
        collectionSlots.Add(Instantiate(slotPrefab));
        collectionSlots[collectionSlots.Count - 1].transform.parent = grid.transform;
        Button collectionBtn = collectionSlots[collectionSlots.Count - 1].GetComponent<Button>();
        collectionBtn.onClick.RemoveAllListeners();
        collectionBtn.onClick.AddListener(() => {
            OpenCollectionPage(entry);
            curCollectionIndex = entry.Index;
        });
        Image collectionImage = collectionSlots[collectionSlots.Count - 1].GetComponent<Image>();
        LoadRawImage(entry, collectionImage, entry.Index);
        LoadSprite(collectionImage, entry.Index);
    }

    /// <summary>
    /// Create new item/collection
    /// </summary>
    public void CreateNewCollection()
    {
        OpenCollectionPage();

        curCollectionIndex= entries.Count;

        collectImageHolder.sprite = defaultImage;
        titleInput.text = "";
        curImageFilePath = "";
        curRecordingFilePath = "";
        noteInput.text = "";
    }

    /// <summary>
    /// Push the iem into the list and save to json file
    /// </summary>
    public void ModifyTheList()     
    {
        //entries.Add(new InputEntry(entries.Count, titleInput.text, imageFilePath, string recordingFilePath, noteInput));

        // Old collection is modified
        if (curCollectionIndex < entries.Count && curCollectionIndex >= 0)
        {
            entries[curCollectionIndex].Title = titleInput.text;
            entries[curCollectionIndex].ImageFilePath= curImageFilePath;
            entries[curCollectionIndex].RecordingFilePath= curRecordingFilePath;
            entries[curCollectionIndex].Notes = noteInput.text;
            LoadRawImage(entries[curCollectionIndex], collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
            LoadSprite(collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
        }
        // New item is added
        else
        {
            entries.Add(new InputEntry(entries.Count, titleInput.text, curImageFilePath, curRecordingFilePath, noteInput.text));
            UpdateInventory(entries[entries.Count - 1]);
        }

        FileHandler.SaveToJSON<InputEntry>(entries, filename);
    }

    /// <summary>
    /// Delte the item from list and json file
    /// </summary>
    public void Delete()
    {

        // Back to inventory Page
        CloseCollectionPage();

        if (curCollectionIndex >= entries.Count)
        {
            return;
        }

        // Remove the entries and collection in file system
        entries.RemoveAt(curCollectionIndex);
        collectionSprite.RemoveAt(curCollectionIndex);

        for (int i = curCollectionIndex; i < entries.Count; i++)
        {
            Debug.Log("Update the index " + entries[i].Index + " to index " + i);
            entries[i].Index = i;
        }
        FileHandler.SaveToJSON<InputEntry>(entries, filename);

        Destroy(collectionSlots[curCollectionIndex]);
        collectionSlots.RemoveAt(curCollectionIndex);
    }

    /// <summary>
    /// Check if the item is saved
    /// </summary>
    public void CheckCloseUnsaved()
    {
        // New item
        if (curCollectionIndex == entries.Count)
        {
            Debug.Log("New things " + curCollectionIndex);
            if (titleInput.text != "" || curImageFilePath != "" || noteInput.text != "")
            {
                unsavedConfirmationPage.SetActive(true);
            }
            else
            {
                CloseCollectionPage();
            }
        }
        // Old item unsaved
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
                CloseCollectionPage();
            }
        }
    }


    /// <summary>
    /// Open the collection page and load item info
    /// </summary>
    /// <param name="entry"></param>
    public void OpenCollectionPage(InputEntry entry = null)
    {
        collectionPage.SetActive(true);
        inventoryPage.SetActive(false);

        if (entry != null)
        {
            LoadInfoToCollectionPage(entry);
        }
    }

    /// <summary>
    /// Close the collection page and open the inventory page
    /// </summary>
    public void CloseCollectionPage() 
    {
        collectionPage.SetActive(false);
        inventoryPage.SetActive(true);
    }


    /// <summary>
    /// Load Sprite for item
    /// </summary>
    /// <param name="holder">where to load</param>
    /// <param name="index">index in the sprite list</param>
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
            if (s.rect.width < s.rect.height)
            {
                //s.ro
            }
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

        //string writePath = Application.persistentDataPath + $"/test_{entries.Count - 1}.png";
        string writePath = Application.persistentDataPath + $"/{FileHandler.GenerateUniqueId()}.png";
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


}