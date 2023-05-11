using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PromptGeneration : MonoBehaviour
{
    public LocalizeStringEvent localizedStringEvent;
    public LocalizedStringTable table;

    private void Awake()
    {
    }

    public void GeneratePrompt(int index)
    {
        localizedStringEvent.SetEntry(index.ToString());
    }

    //private void OnEnable()
    //{
   
    //    int randomIndex = UnityEngine.Random.Range(1, table.GetTable().SharedData.Entries.Count + 1);
    //    localizedStringEvent.SetEntry(randomIndex.ToString());
    //}
}
