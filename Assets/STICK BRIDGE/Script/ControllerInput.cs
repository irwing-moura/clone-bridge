using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    public GameObject tutorialObj;
    public void BeginStretch()
    {
        tutorialObj.SetActive(false);
        LevelManager.Instance.BeginStretchBridge();
    }

    public void StopStretch()
    {
        LevelManager.Instance.StopStretchBridge();
    }
}
