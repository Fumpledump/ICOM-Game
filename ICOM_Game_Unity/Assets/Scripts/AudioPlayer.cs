using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioPlayer : MonoBehaviour
{
    public GameObject audioRecorder;

    public AudioSource audioSource;
    private AudioClip recordedAudio;
    public string audioPath;
    public string audioName;

    private Coroutine audioLoadRoutine;

    public bool audioIntialLoad;

    private void Awake()
    {
        // Get Name of Audio File
        audioName = audioRecorder.GetComponent<Recorder>().fileName + ".wav";

        audioSource = gameObject.AddComponent<AudioSource>();
        audioPath = "file://" + Application.streamingAssetsPath;
    }

    private void Start()
    {
        audioLoadRoutine = StartCoroutine(LoadAudioRoutine());
        audioIntialLoad = true;
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
            }
        }

        StopCoroutine(audioLoadRoutine);
    }

    public void LoadAudio()
    {
        if (audioIntialLoad)
        {
            audioLoadRoutine = StartCoroutine(LoadAudioRoutine());
        }
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }
}
