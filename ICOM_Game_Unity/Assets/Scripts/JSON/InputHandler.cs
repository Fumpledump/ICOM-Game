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
    [SerializeField] bool curFavorited;
    bool imageChanged = false;

    public List<InputEntry> entries = new List<InputEntry>();
    public List<GameObject> collectionSlotsStacks = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    public List<GameObject> collectionSlots = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    List<GameObject> slotStacks = new List<GameObject>(); // grid slots, needs to be replaced by game object in the future
    public List<Sprite> collectionSprite = new List<Sprite>();
    public int curCollectionIndex = -1;

    public int totalEntries; // Total amount of entries in the game.

    // Canvas Part
    public GameObject collectionPage;
    public GameObject inventoryPage;
    public ScrollRect scrollView;
    public GameObject unsavedConfirmationPage;
    public GameObject deleteConfirmPage;
    public GameObject grid;
    public TextMeshProUGUI console;
    public Texture2D texture;
    public Image collectImageHolder;
    public GameObject favaritedButton;
    public GameObject languagePage;
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

        if (!File.Exists(Application.persistentDataPath + $"/{filename}"))
        {
            languagePage.SetActive(true);
        }

        //if(!PlayerPrefs.HasKey("NotFirstTimeOpening"))
        //{
        //    languagePage.SetActive(true);
        //}
        //else
        //{
        //    PlayerPrefs.SetInt("NotFirstTimeOpening", 0);
        //}
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
            LoadRawImage(entry, i);
            //StartCoroutine(LoadRawImageAsync(entry, i));
            UpdateSlotStack(i, entry);
        }

        totalEntries = entries.Count;
    }


    public void UpdateSlotStack(int index, InputEntry entry)
    {
        if (index % 3 == 0 || index == 0) // Create New Slot Stack
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

    /// <summary>
    /// Load the item/collection into the collection page
    /// </summary>
    /// <param name="collection">loaded collection</param>
    public void LoadInfoToCollectionPage(InputEntry collection)
    {
        Debug.Log("Loading Slot: " + curCollectionIndex);

        imageChanged = false;
        curCollectionIndex = collection.Index;
        curImageFilePath = collection.ImageFilePath;
        curRecordingFileName = collection.RecordingFileName;
        titleInput.text = collection.Title;
        noteInput.text = collection.Notes;
        curFavorited = collection.Favorited;

        if (collection.RecordingClipLength >= 0)
        {
            recorder.fileName = collection.RecordingFileName;
            audioPlayer.LoadSlotAudio(curCollectionIndex, entries[curCollectionIndex].RecordingClipLength);
            audioPlayer.Playable();
        }

        if (collection.Favorited)
        {
            favaritedButton.gameObject.SetActive(true);
        }
        else 
        {
            favaritedButton.gameObject.SetActive(false);
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
        //collectionSlotsStacks.Clear();
        //Transform gridTrans = grid.GetComponent<Transform>();
        //foreach (Transform child in gridTrans)
        //{
        //    GameObject.Destroy(child.gameObject);
        //}
        //totalEntries = entries.Count;
        //for (int i = 0; i < entries.Count; i++)
        //{
        //    UpdateSlotStack(i, entries[i]);
        //}


        if (totalEntries % 3 == 0) // Create New Slot Stack
        {
            GameObject newSlotStack = Instantiate(slotStackPrefab);
            collectionSlotsStacks.Add(newSlotStack);
            newSlotStack.transform.SetParent(grid.transform);

            // Create New Slot
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(newSlotStack.transform);

            // Load the image into the list
            //LoadRawImage(entry, entry.Index);
            collectionSprite.Insert(entry.Index, collectImageHolder.sprite);


            // Add to Slot Stack
            newSlotStack.GetComponent<Slot>().slots.Add(newSlot);
            newSlotStack.GetComponent<Slot>().entries.Add(entry);

        }
        else // Add to Slot Stack
        {
            // Create New Slot
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(collectionSlotsStacks[collectionSlotsStacks.Count - 1].transform);

            // Load the image into the list
            //LoadRawImage(entry, entry.Index);
            collectionSprite.Insert(entry.Index, collectImageHolder.sprite);

            // Add to Slot Stack
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().slots.Add(newSlot);
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().entries.Add(entry);
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().UpdateSlots();

            // Bugfix Position
            newSlot.transform.localPosition = Vector3.zero;

        }
        //LoadRawImage(entry,entry.Index);
        //LoadSprite(newSlot.GetComponent<Image>(), entry.Index);
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

        curFavorited = false;
        favaritedButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Push the iem into the list and save to json file
    /// </summary>
    public void ModifyTheList()     
    {
        //entries.Add(new InputEntry(entries.Count, titleInput.text, imageFilePath, string RecordingFileName, noteInput));

       
        if (imageChanged)
        {
            if (File.Exists(curImageFilePath))
            {
                File.Delete(curImageFilePath);
            }
            WriteImageIntoFolder();
        }

        // Old collection is modified
        if (curCollectionIndex < entries.Count && curCollectionIndex >= 0)
        {
            entries[curCollectionIndex].Title = titleInput.text;
            entries[curCollectionIndex].ImageFilePath= curImageFilePath;
            entries[curCollectionIndex].RecordingFileName = recorder.fileName;
            entries[curCollectionIndex].RecordingClipLength = audioPlayer.recordedClipLength;
            entries[curCollectionIndex].Notes = noteInput.text;
            entries[curCollectionIndex].Favorited = curFavorited;
            collectionSprite.RemoveAt(curCollectionIndex);
            collectionSprite.Insert(curCollectionIndex, collectImageHolder.sprite);
            collectionSlotsStacks[curCollectionIndex / 3].GetComponent<Slot>().UpdateSlots();
        }
        // New item is added
        else
        {
            entries.Add(new InputEntry(entries.Count, titleInput.text, curImageFilePath, recorder.fileName, audioPlayer.recordedClipLength, noteInput.text, curFavorited));
            Debug.Log(curCollectionIndex);
            UpdateInventory(entries[entries.Count - 1]);
            collectionSlotsStacks[collectionSlotsStacks.Count - 1].GetComponent<Slot>().UpdateSlots();
        }

        Debug.Log("Scroll View Position Before:" + scrollView.verticalNormalizedPosition);

        // Move Inventory Rows Down
        scrollView.verticalNormalizedPosition = 1.5f;

        Debug.Log("Scroll View Position After:" + scrollView.verticalNormalizedPosition);


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

        // Delete the image file in the folder
        string filepath = entries[curCollectionIndex].ImageFilePath;
        if (File.Exists(filepath))
        {
            Debug.Log("Try to delete the file in the foldr");
            File.Delete(filepath);
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

        //Destroy(collectionSlotsStacks[curCollectionIndex / 3].GetComponent<Slot>().slots[curCollectionIndex % 3]);
        //collectionSlotsStacks[curCollectionIndex / 3].GetComponent<Slot>().slots.RemoveAt(curCollectionIndex % 3);
        collectionSlotsStacks[curCollectionIndex / 3].GetComponent<Slot>().DeleteSlot(curCollectionIndex % 3);

        // Update Slots
        //for (int i = 0; i < collectionSlotsStacks.Count; i++)
        //{
        //    collectionSlotsStacks[0].GetComponent<Slot>().UpdateSlots();
        //}
        collectionSlotsStacks.Clear();
        Transform gridTrans = grid.GetComponent<Transform>();
        foreach (Transform child in gridTrans)
        {
            GameObject.Destroy(child.gameObject);
        }
        totalEntries = entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            UpdateSlotStack(i, entries[i]);
        }
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
            if (titleInput.text != "" || imageChanged || noteInput.text != "")
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
                imageChanged ||
                noteInput.text != entries[curCollectionIndex].Notes ||
                curFavorited != entries[curCollectionIndex].Favorited)
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

    public void FavoriteCollection() 
    {
        favaritedButton.SetActive(true);
        curFavorited = true;
    }

    public void UnFavoriteCollection()
    {
        favaritedButton.SetActive(false);
        curFavorited = false;
    }

    /// <summary>
    /// This method will be called when the player load image but don't want to save it
    /// Still need to consider when user load multiple image but did not save
    /// </summary>
    public void DeleteUnsavedImage() 
    {
        //Debug.Log("current collection file path:" + entries[curCollectionIndex].ImageFilePath);
        Debug.Log("Current image file path:" + curImageFilePath);
        if(File.Exists(curImageFilePath))
        {
            File.Delete(curImageFilePath);
        }
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

    public void LoadRawImage(InputEntry collection, int index)
    {
        curImageFilePath = collection.ImageFilePath;
        if (collection.ImageFilePath != "")
        {
            Sprite s = IMG2Sprite.LoadNewSprite(collection.ImageFilePath);
            collectionSprite.Insert(index, s);
        }
        else 
        {
            collectionSprite.Insert(index, defaultImage);
        }
    }

    public IEnumerator LoadRawImageAsync(InputEntry collection, int index)
    {
        curImageFilePath = collection.ImageFilePath;
        if (collection.ImageFilePath != "")
        {
            // Load the image asynchronously using the WWW class
            using (WWW www = new WWW(collection.ImageFilePath))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    // Create a new Texture2D from the downloaded data
                    Texture2D texture = new Texture2D(2, 2);
                    www.LoadImageIntoTexture(texture);
                    // Create a new Sprite from the Texture2D
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    collectionSprite.Insert(index, sprite);
                }
                else
                {
                    Debug.LogError("Error loading image: " + www.error);
                    collectionSprite.Insert(index, defaultImage);
                }
            }
        }
        else
        {
            collectionSprite.Insert(index, defaultImage);
        }
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

                Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                collectImageHolder.sprite = s;

                imageChanged = true;

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

                Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                collectImageHolder.sprite = s;

                imageChanged = true;
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    private void WriteImageIntoFolder()
    {
        string writePath = Application.persistentDataPath + $"/{FileHandler.GenerateUniqueId()}.png";
        curImageFilePath = writePath;
        byte[] byteArray = File.ReadAllBytes(imagePath);
        File.WriteAllBytes(writePath, byteArray);
    }
}