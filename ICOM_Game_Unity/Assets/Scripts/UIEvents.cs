using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inlcude all UI event functions
/// </summary>
public class UIEvents : MonoBehaviour
{
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
