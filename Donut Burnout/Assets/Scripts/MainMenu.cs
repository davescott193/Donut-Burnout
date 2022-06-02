using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [Header("Menu holds objects and does tasks related to the UI. Hover over anything to read info")]

    [Tooltip("Is the object called Start Menu that holds the initial buttons")]
    public GameObject StartMenuGameObject;

    [Tooltip("Is the object called Settings Menu that holds buttons that tweak the game")]
    public GameObject SettingsMenuGameObject;

    [Tooltip("Is the object called Controls Menu that displays a map of how controls work")]
    public GameObject ControlsMenuGameObject;

    private void Awake()
    {
        instance = this;
    }
}
