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
    public static InputHandler instance;

    public GameObject slotPrefab;
    public GameObject slotStackPrefab;
    public Sprite defaultImage;

    // Mic variables
    public GameObject mic;
    private Recorder recorder;
    private AudioPlayer audioPlayer;

    [SerializeField] TMP_InputField titleInput;
    [SerializeField] string curImageFilePath;
    [SerializeField] string curRecordingFileName;
    [SerializeField] TMP_InputField noteInput;
    [SerializeField] string filename;

    public List<InputEntry> entries = new List<InputEntry>();
    List<GameObject> collectionSlotsStacks = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    List<GameObject> collectionSlots = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    List<GameObject> slotStacks = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    List<Sprite> collectionSprite = new List<Sprite>();
    public int curCollectionIndex = -1;

    public int totalEntries; // Total amount of entries in the game.

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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

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
            if (i % 3 == 0 || i == 0) // Create New Slot Stack
            {
                GameObject newSlotStack = Instantiate(slotStackPrefab);
                collectionSlotsStacks.Add(newSlotStack);
                newSlotStack.transform.SetParent(grid.transform);

                // Create New Slot
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(newSlotStack.transform);

                // Add to Slot Stack
                newSlotStack.GetComponent<Slot>().slots.Add(newSlot);
                newSlotStack.GetComponent<Slot>().entries.Add(entry);
                newSlotStack.GetComponent<Slot>().UpdateSlots();
            }
            else // Add to Slot Stack
            {
                // Create New Slot
                GameObject newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(collectionSlotsStacks[collectionSlotsStacks.Count - 1].transform);

                // Add to Slot Stack
                collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().slots.Add(newSlot);
                collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().entries.Add(entry);
                collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().UpdateSlots();
            }
        }

        totalEntries = entries.Count;
    }

    /// <summary>
    /// Load the item/collection into the collection page
    /// </summary>
    /// <param name="collection">loaded collection</param>
    public void LoadInfoToCollectionPage(InputEntry collection)
    {
        Debug.Log("Loading Slot: " + curCollectionIndex);

        curCollectionIndex = collection.Index;
        curImageFilePath = collection.ImageFilePath;
        curRecordingFileName = collection.RecordingFileName;
        titleInput.text = collection.Title;
        noteInput.text = collection.Notes;

        if (collection.RecordingClipLength >= 0)
        {
            recorder.fileName = collection.RecordingFileName;
            audioPlayer.LoadSlotAudio(curCollectionIndex, entries[curCollectionIndex].RecordingClipLength);
            audioPlayer.Playable();
        }

        LoadSprite(collectImageHolder, collection.Index);
        Debug.Log("LoadName " + curCollectionIndex);
    }

    /// <summary>
    /// Update Inventory when new item is added
    /// This will be rework when there is real inventory instead of grid
    /// </summary>
    /// <param name="entry">added item</param>
    public void UpdateInventory(InputEntry entry)
    {
        if (totalEntries % 3 == 0) // Create New Slot Stack
        {
            GameObject newSlotStack = Instantiate(slotStackPrefab);
            collectionSlotsStacks.Add(newSlotStack);
            newSlotStack.transform.SetParent(grid.transform);

            // Create New Slot
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(newSlotStack.transform);

            // Add to Slot Stack
            newSlotStack.GetComponent<Slot>().slots.Add(newSlot);
            newSlotStack.GetComponent<Slot>().entries.Add(entry);
        }
        else // Add to Slot Stack
        {
            // Create New Slot
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(collectionSlotsStacks[collectionSlotsStacks.Count - 1].transform);

            // Add to Slot Stack
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().slots.Add(newSlot);
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().entries.Add(entry);
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().UpdateSlots();

            // Bugfix Position
            newSlot.transform.localPosition = Vector3.zero;
        }

        totalEntries++;
    }

    /// <summary>
    /// Create new item/collection
    /// </summary>
    public void CreateNewCollection()
    {
        audioPlayer.FirstRecord();

        OpenCollectionPage();

        curCollectionIndex = entries.Count;

        collectImageHolder.sprite = defaultImage;
        titleInput.text = "";
        curImageFilePath = "";
        curRecordingFileName = "";
        noteInput.text = "";
    }

    /// <summary>
    /// Push the iem into the list and save to json file
    /// </summary>
    public void ModifyTheList()     
    {
        //entries.Add(new InputEntry(entries.Count, titleInput.text, imageFilePath, string RecordingFileName, noteInput));

        // Old collection is modified
        if (curCollectionIndex < entries.Count && curCollectionIndex >= 0)
        {
            entries[curCollectionIndex].Title = titleInput.text;
            entries[curCollectionIndex].ImageFilePath= curImageFilePath;
            entries[curCollectionIndex].RecordingFileName = recorder.fileName;
            entries[curCollectionIndex].RecordingClipLength = audioPlayer.recordedClipLength;
            entries[curCollectionIndex].Notes = noteInput.text;
            LoadRawImage(entries[curCollectionIndex], collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
            LoadSprite(collectionSlots[curCollectionIndex].GetComponent<Image>(), curCollectionIndex);
        }
        // New item is added
        else
        {
            entries.Add(new InputEntry(entries.Count, titleInput.text, curImageFilePath, recorder.fileName, audioPlayer.recordedClipLength, noteInput.text));
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
    // Button handlers From Native Toolkit
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
    // Callbacks From Native ToolKit
    //=============================================================================

    void ScreenshotSaved(string path)
    {
        console.text += "\n" + "Screenshot saved to: " + path;
    }

    void ImageSaved(string path)
    {
        imagePath = path;
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

        string path = "C:\\Users\\RuniJiang\\Downloads\\IMG_7000-ID.jpg";
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

    public void OnTakePictureTest()
    {
        TakePicture(512);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                imagePath = path;
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //// Assign texture to a temporary quad and destroy it after 5 seconds
                //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                //quad.transform.forward = Camera.main.transform.forward;
                //quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                //Material material = quad.GetComponent<Renderer>().material;
                //if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                //    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                //material.mainTexture = texture;

                //Destroy(quad, 5f);

                //// If a procedural texture is not destroyed manually, 
                //// it will only be freed after a scene change
                //Destroy(texture, 5f);

                Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                collectImageHolder.sprite = s;
                string writePath = Application.persistentDataPath + $"/{FileHandler.GenerateUniqueId()}.png";
                curImageFilePath = writePath;
                byte[] byteArray = File.ReadAllBytes(imagePath);
                File.WriteAllBytes(writePath, byteArray);
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    public void OnPickTest()
    {
        PickImage(512);
    }

    private void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                imagePath = path;
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //// Assign texture to a temporary quad and destroy it after 5 seconds
                //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                //quad.transform.forward = Camera.main.transform.forward;
                //quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                //Material material = quad.GetComponent<Renderer>().material;
                //if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                //    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                //material.mainTexture = texture;

                //Destroy(quad, 5f);

                //// If a procedural texture is not destroyed manually, 
                //// it will only be freed after a scene change
                //Destroy(texture, 5f);

                Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                collectImageHolder.sprite = s;
                string writePath = Application.persistentDataPath + $"/{FileHandler.GenerateUniqueId()}.png";
                curImageFilePath = writePath;
                byte[] byteArray = File.ReadAllBytes(imagePath);
                File.WriteAllBytes(writePath, byteArray);
            }
        });

        Debug.Log("Permission result: " + permission);
    }
}