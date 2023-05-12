using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;
    public bool firstTime = false;
    [SerializeField] private List<Button> lanaguageButtons;

    private void Awake()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
        if(!firstTime)
        {
            for (int i = 0; i < lanaguageButtons.Count; i++)
            {
                if (i != ID)
                {
                    lanaguageButtons[i].interactable = true;
                }
                else
                {
                    lanaguageButtons[i].interactable = false;
                }
            }
        }
    }
    public void ChangeLocale(int localeID)
    {
        if (active == true)
            return;
        StartCoroutine(SetLocale(localeID));
    }
    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }
}
