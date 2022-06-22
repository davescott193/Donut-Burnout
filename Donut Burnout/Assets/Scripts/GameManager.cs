using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
//using UnityEngine.Rendering.PostProcessing;


/*
 
 Bachelor of Software Engineering
 Media Design School
 Auckland
 New Zealand

 (c) 2022 Media Design School

 File Name    : GameManager.cs
 Description  : holds vitial functions and loading scenes
 Author       : Allister Hamilton

*/


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Drag your mp3 or wav clips into here and the game will do the rest")]
    public AudioClip ButtonDownSound;
    public AudioClip ButtonUpSound;
    [Space(10)]
    public AudioClip PlayerFootStepSound;
    public AudioClip PlayerJumpSound;
    [Space(10)]
    public AudioClip CustomerEntersSound;
    public AudioClip CustomerOrdersSound;
    public AudioClip CustomerReceivesSound;
    public AudioClip CustomerEatsSound;
    public AudioClip CustomerPlacesPlateDownSound;
    [Space(10)]
    public AudioClip LoadingSound;
    [Space(10)]
    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    [Header("Other variables to just leave alone")]
    public AudioPool MusicPool;
    public AudioPool SoundPool;
    public Animation SceneAnimation;
    Coroutine LoadingCoroutine;
    // [HideInInspector]
    public List<GameObject> FoodList = new List<GameObject>();

    [System.Serializable]
    public class ToggleData
    {
        public string ToggleName;
        public Color ToggleColor;
    }

    private void Awake()
    {

        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (PlayerPrefs.GetInt("First Time") == 0)
            {
                PlayerPrefs.SetInt("First Time", 1);

                PlayerPrefs.SetFloat("Music", .4f);
                PlayerPrefs.SetFloat("Sound", 1);
                PlayerPrefs.SetFloat("Camera Speed", 2);

                PlayerPrefs.SetInt("Highest Level", 1);

                PlayerPrefs.SetInt("Performance", 1);
                PlayerPrefs.SetInt("Shadows", 1);
            }

#if UNITY_EDITOR
            string[] searchedAssetsArray = AssetDatabase.FindAssets("", new[] { "Assets/Resources/Food" });

            for (int i = 0; i < searchedAssetsArray.Length; i++)
            {
                string assetPathString = AssetDatabase.GUIDToAssetPath(searchedAssetsArray[i]);
                FoodList.Add((GameObject)AssetDatabase.LoadAssetAtPath(assetPathString, typeof(GameObject)));
            }
#endif
        }

        firstBool = true;
        ChangeScene(0);
    }
    bool firstBool;
    public void ChangeScene(int sceneNumber = -1)
    {
        if (sceneNumber == -1)        //doing sceneindex as -1 will assume its just restarting scene
            sceneNumber = SceneManager.GetActiveScene().buildIndex;


        PauseChange(true);

        if (LoadingCoroutine != null)
        {
            StopCoroutine(LoadingCoroutine);
        }

        LoadingCoroutine = StartCoroutine(ChangeSceneIEnumerator(sceneNumber));
    }

    public static void PauseChange(bool Show)
    {
        Time.timeScale = Show ? 1 : 0;
    }

    public static void CursorChange(bool Show)
    {
        Cursor.visible = Show;
        Cursor.lockState = Show ? CursorLockMode.None : CursorLockMode.Locked;
    }


    public static List<string> ReturnLayerMaskNames(LayerMask MaskRequested)
    {
        List<string> layers = new List<string>();
        var bitmask = MaskRequested.value;

        for (int i = 0; i < 32; i++)
        {
            if (((1 << i) & bitmask) != 0)
            {
                layers.Add(LayerMask.LayerToName(i));
            }
        }
        return layers;
    }

    IEnumerator ChangeSceneIEnumerator(int sceneNumber)
    {
        AnimationState animationState = AnimationChangeDirection(GetComponent<Animation>(), "Load", true, firstBool);

        GameManager.instance.MusicPool.DynamicVolumeChange(-1);

        yield return new WaitUntil(() => !animationState.enabled);

        GameManager.instance.SoundPool.PlaySound(GameManager.instance.LoadingSound);

        if (!firstBool)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNumber);

            yield return new WaitUntil(() => operation.isDone);
        }
        firstBool = false;
        animationState = AnimationChangeDirection(GetComponent<Animation>(), "Load", false);
        animationState.time = 0.5f;
        CursorChange(sceneNumber == 0);
        MusicPool.PlayMusic(sceneNumber == 0 ? MenuMusic : GameMusic);
        LoadingCoroutine = null;

        // GameManager.instance.MusicPool.PlayMusic(SceneMusicList[sceneNumber]);
    }


    public static float ReturnThresholds(float Size, int MaxSize, int MinSize = 0, bool WrapAround = true)
    {
        //Debug.Log(MinSize + " < " + Size + " < " + MaxSize);

        for (int i = 0; i < 1; i++)
        {
            if (Size < MinSize)
            {
                //-1 = 4 + (0 - - 1)
                if (WrapAround)
                {
                    Size = MaxSize + (MinSize + Size) + 1;
                    i = -1;
                }
                else
                {
                    Size = MinSize;
                }

                continue;
            }

            if (Size > MaxSize)
            {
                if (WrapAround)
                {
                    Size = MinSize + (Size - MaxSize) - 1;
                    i = -1;
                }
                else
                {
                    Size = MaxSize;
                }

                continue;
            }
        }

        return Size;
    }


    public static int ReturnBitShift(string[] LayerNames = null, bool Exclude = false)
    {
        if (LayerNames == null)
        {
            LayerNames = new string[] { "Default" };
        }

        int layerMask = 0;

        for (int i = 0; i < LayerNames.Length; i++)
        {
            layerMask = layerMask | 1 << LayerMask.NameToLayer(LayerNames[i]);
        }

        if (Exclude)
        {
            layerMask = ~layerMask;
        }

        return layerMask;
    }

    public static void ResetChosenHolder(Transform Holder, int StartNumber = 0)
    {
        List<Transform> AllChilds = new List<Transform>();

        for (int i = StartNumber; i < Holder.childCount; i++)
        {
            AllChilds.Add(Holder.GetChild(i));
        }

        for (int i = 0; i < AllChilds.Count; i++)
        {
            Destroy(AllChilds[i].gameObject);
            AllChilds[i].gameObject.SetActive(false);
            AllChilds[i].SetParent(null);
        }
    }

    public static void ChangeAnimationLayers(Animation ChosenAnimation)
    {
        if (ChosenAnimation)
        {
            int Index = 0;
            foreach (AnimationState clip in ChosenAnimation)
            {
                ChosenAnimation[clip.name].layer = Index;
                ChosenAnimation[clip.name].speed = 1;
                Index += 1;
            }
        }
    }

    public static AnimationState AnimationChangeDirection(Animation ChosenAnimation, string AnimationName = "", bool Forward = true, bool Instant = false)
    {
        if (ChosenAnimation)
        {
            //Debug.Log(ChosenAnimation.name + " " + AnimationName);
            //AnimationName = "" will default to the first animation

            if (ChosenAnimation.GetClip(AnimationName) || ChosenAnimation.clip)
            {

                if (!ChosenAnimation.GetClip(AnimationName) && ChosenAnimation.clip)
                {
                    if (AnimationName != "")
                    {
                        Debug.LogError("Animation Missing for " + AnimationName + " so will be defaulting to first clip");
                    }

                    AnimationName = ChosenAnimation.clip.name;
                }


                //Time Types:

                //0 = Will continue from where it was interupted
                //1 = Will start from fresh
                //2 = Will instantly reach one side of the animation


                if (Forward)
                {

                    if (Instant)
                    {
                        ChosenAnimation[AnimationName].time = ChosenAnimation[AnimationName].length;
                    }

                    ChosenAnimation[AnimationName].speed = 1;
                }
                else
                {
                    if (ChosenAnimation[AnimationName].time == 0)
                    {
                        ChosenAnimation[AnimationName].time = ChosenAnimation[AnimationName].length;
                    }

                    if (Instant)
                    {
                        ChosenAnimation[AnimationName].time = 0;
                    }


                    ChosenAnimation[AnimationName].speed = -1;
                }
                float timeFloat = ChosenAnimation[AnimationName].time;

                ChosenAnimation.Play(AnimationName);
                ChosenAnimation[AnimationName].time = timeFloat;
                return ChosenAnimation[AnimationName];
            }
        }

        Debug.LogWarning("No animation found");
        return new AnimationState();
    }
}
