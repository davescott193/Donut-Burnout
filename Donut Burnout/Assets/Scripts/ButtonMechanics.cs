using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
 
 Bachelor of Software Engineering
 Media Design School
 Auckland
 New Zealand

 (c) 2021 Media Design School

 File Name    : ButtonMechanics.cs
 Description  : mechanics for what buttons do
 Author       : Allister Hamilton

*/


public class ButtonMechanics : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Toggle below to activate button function. Hover over anything to read info. Ususally a button will only have one toggle ticked.")]

    [Header("Start Menu Buttons")]

    public bool PlayGameBool;

    [Tooltip("Opens the controls Menu")]
    public bool OpenControlsBool;

    [Tooltip("Opens the settings Menu")]
    public bool OpenSettingsBool;


    [Tooltip("Fully closes the application. It won't work in Unity Editor, only when built.")]
    public bool ExitGameBool;

    [Header("Controls Menu Buttons")]

    [Tooltip("Close the Controls Menu")]
    public bool CloseControlsBool;

    [Header("Settings Menu Buttons")]

    [Tooltip("Close the settings Menu")]
    public bool CloseSettingsBool;

    [Tooltip("Toggles the debug on and off to show whats under the hood for developing")]
    public bool DebugToggleBool;

    [Tooltip("Toggles the shadows on and off")]
    public bool ShadowsToggleBool;

    [Tooltip("Turns off rotating cameras etc to save on fps")]
    public bool PerformanceToggleBool;

    [Tooltip("Sound or Music name")]
    public string PoolName;

    [Tooltip("Speed the camera rotates")]
    public bool CameraSpeedBool;

    void OnEnable()
    {
        if (ShadowsToggleBool)
        {
            if (PlayerPrefs.GetInt("Shadows") == 1)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                transform.GetChild(1).GetComponent<Text>().text = "Shadows: ON";
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.All;
                transform.GetChild(1).GetComponent<Text>().text = "Shadows: OFF";
            }
        }

        if (DebugToggleBool)
        {
            if (PlayerPrefs.GetInt("Debug") == 1)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                transform.GetChild(1).GetComponent<Text>().text = "Debug Mode: ON";
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.All;
                transform.GetChild(1).GetComponent<Text>().text = "Debug Mode: OFF";
            }
        }

        if (PerformanceToggleBool)
        {
            if (PlayerPrefs.GetInt("Performance") == 1)
            {
                transform.GetChild(1).GetComponent<Text>().text = "Performance: ON";
            }
            else
            {
                transform.GetChild(1).GetComponent<Text>().text = "Performance: OFF";
            }
        }

        if (PoolName != "")
        {
            transform.GetChild(0).GetComponent<Slider>().value = PlayerPrefs.GetFloat(PoolName);
        }

        if (CameraSpeedBool)
        {
            transform.GetChild(0).GetComponent<Slider>().value = PlayerPrefs.GetFloat("Camera Speed");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        float Scale = 1f;
        transform.localScale = new Vector3(Scale, Scale, Scale);

        if (GetComponent<Animation>())
            GameManager.AnimationChangeDirection(GetComponent<Animation>(), "Select", false).wrapMode = WrapMode.Once;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        float Scale = 1.02f;
        transform.localScale = new Vector3(Scale, Scale, Scale);

        if (GetComponent<Animation>())
            GameManager.AnimationChangeDirection(GetComponent<Animation>(), "Select").wrapMode = WrapMode.PingPong;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        float Scale = .98f;
        transform.localScale = new Vector3(Scale, Scale, Scale);
        GameManager.instance.SoundPool.PlaySound(GameManager.instance.ButtonDownSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.SoundPool.PlaySound(GameManager.instance.ButtonUpSound);
        OnPointerEnter(eventData);
        DoFunction();
    }

    public void DoFunction()
    {
        if (PoolName != "")
        {
            PlayerPrefs.SetFloat(PoolName, transform.GetChild(0).GetComponent<Slider>().value);

            GameManager.instance.MusicPool.RefreshAudioVolumes();
            GameManager.instance.SoundPool.RefreshAudioVolumes();
        }

        if (CameraSpeedBool)
        {
            PlayerPrefs.SetFloat("Camera Speed", transform.GetChild(0).GetComponent<Slider>().value);
        }

        if (OpenSettingsBool)
        {
            GameManager.AnimationChangeDirection(MainMenu.instance.GetComponent<Animation>(), "Settings Menu Change");
        }

        if (CloseSettingsBool)
        {
            GameManager.AnimationChangeDirection(MainMenu.instance.GetComponent<Animation>(), "Settings Menu Change", false);
        }

        if (OpenControlsBool)
        {
            GameManager.AnimationChangeDirection(MainMenu.instance.GetComponent<Animation>(), "Controls Menu Change");
        }

        if (CloseControlsBool)
        {
            GameManager.AnimationChangeDirection(MainMenu.instance.GetComponent<Animation>(), "Controls Menu Change", false);
        }
        if (ExitGameBool)
        {
            Application.Quit();
        }

        if (PlayGameBool)
        {
            GameManager.instance.ChangeScene(1);
        }

        if (PerformanceToggleBool)
        {
            PlayerPrefs.SetInt("Performance", PlayerPrefs.GetInt("Performance") == 1 ? 0 : 1);
            OnEnable();
        }

        if (DebugToggleBool)
        {
            PlayerPrefs.SetInt("Debug", PlayerPrefs.GetInt("Debug") == 1 ? 0 : 1);
            OnEnable();
        }

        if (ShadowsToggleBool)
        {
            PlayerPrefs.SetInt("Shadows", PlayerPrefs.GetInt("Shadows") == 1 ? 0 : 1);
            OnEnable();
        }
    }
}
