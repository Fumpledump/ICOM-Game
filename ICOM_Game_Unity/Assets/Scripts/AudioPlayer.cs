using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    public GameObject audioRecorder;

    public AudioSource audioSource;
    private AudioClip recordedAudio;
    public string audioPath;
    public string audioName;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;

    public float recordedClipLength;
    public bool audioLoaded;

    public float debugTime;
    public float endClipThreshold;

    private Coroutine audioLoadRoutine;

    private void Update()
    {
        if (audioLoaded)
        {
            playButton.SetActive(!audioSource.isPlaying);
            pauseButton.SetActive(audioSource.isPlaying);

            debugTime = audioSource.time;

            if (audioSource.isPlaying)
            {
                audioSlider.value = audioSource.time;
            }

            if (audioSource.time + endClipThreshold >= recordedClipLength)
            {
                audioSource.Pause();
                audioSource.time = 0;
                audioSlider.value = 0;
            }
        }
    }

    private IEnumerator LoadAudioRoutine()
    {
        Debug.Log("Loading File: " + audioPath + "/" + audioName);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioPath + "/" + audioName, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                recordedAudio = DownloadHandlerAudioClip.GetContent(www);
                recordedAudio.name = audioName;
                audioSource.clip = recordedAudio;
                audioSlider.maxValue = recordedClipLength;
                audioLoaded = true;
            }
        }
        StopCoroutine(audioLoadRoutine);
    }

    public void LoadAudio()
    {
        // Get Name of Audio File
        audioName = audioRecorder.GetComponent<Recorder>().fileName + ".wav";

        audioSource = gameObject.AddComponent<AudioSource>();
        audioPath = "file://" + Application.persistentDataPath;


        audioLoadRoutine = StartCoroutine(LoadAudioRoutine());
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void SetAudioAtSlider()
    {
        if (audioSlider.value > audioSource.clip.length)
        {
            Debug.Log("Play Time Specified is long than audio length");
            return;
        }
        else
        {
            audioSource.time = audioSlider.value;
        }
    }
}
