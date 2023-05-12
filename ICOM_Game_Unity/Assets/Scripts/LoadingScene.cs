using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    


    public int sceneID;
    public int waitTime;


    private void Awake()
    {
        
    }
    private void Start()
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        // Start loading the scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation= false;
        // Wait for the scene to finish loading, but also add a delay
        float fakeLoadingDuration = waitTime;
        float startTime = Time.time;
        while (!operation.isDone && Time.time < startTime + fakeLoadingDuration)
        {
            yield return null;
        }

        // If loading is faster than 2 seconds, wait until 2 seconds have elapsed
        if (Time.time < startTime + fakeLoadingDuration)
        {
            yield return new WaitForSecondsRealtime(startTime + fakeLoadingDuration - Time.time);
        }

        // Show fake loading screen for 2 seconds
        // Update fake loading progress bar or animation here
        while (Time.time < startTime + fakeLoadingDuration)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;
        // Hide loading screen or transition to the next scene here
    }

}
