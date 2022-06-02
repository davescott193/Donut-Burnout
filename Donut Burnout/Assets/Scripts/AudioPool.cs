using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 
 Bachelor of Software Engineering
 Media Design School
 Auckland
 New Zealand

 (c) 2022 Media Design School

 File Name    : AudioPool.cs
 Description  : Manages Sound and music
 Author       : Allister Hamilton

*/

public class AudioPool : MonoBehaviour
{
    public string AudioName;

    public bool SkipToNearEnd;

    [Header("Indexes")]
    public int PoolPosition;

    [Header("Lists")]
    public List<AudioData> Audio_List = new List<AudioData>();

    [System.Serializable]
    public class AudioData
    {
        public AudioSource Audio;
        public Transform SpatialTransform;
        public float AudioVolume;
        public float AudioDirection;

        public int Piority;

        public int SceneNumber;

        public float TransitionTime;

        public bool isPaused;
    }

    public void Awake()
    {
        name = AudioName + " Holder";

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < 10; i++)
        {
            AudioSource audioCreated = new GameObject().AddComponent<AudioSource>();
            audioCreated.transform.SetParent(transform);
            audioCreated.loop = false;
            audioCreated.playOnAwake = false;

            audioCreated.name = AudioName + ": " + i.ToString("00");
            //audioCreated.rolloffMode = AudioRolloffMode.Logarithmic;

            //audioCreated.SetCustomCurve(AudioSourceCurveType.CustomRolloff, SetupManager.singleton.animationCurve);

            audioCreated.maxDistance = 30;
            audioCreated.dopplerLevel = 0;
            Audio_List.Add(new AudioData());
            Audio_List[Audio_List.Count - 1].Audio = audioCreated;
        }

        RefreshAudioVolumes();
    }

    private void Update()
    {
        if (SkipToNearEnd)
        {
            SkipToNearEnd = false;
            Audio_List[PoolPosition].Audio.time = Audio_List[PoolPosition].Audio.clip.length - Audio_List[PoolPosition].TransitionTime - 1f;
        }

        for (int i = 0; i < Audio_List.Count; i++)
        {
            if (Audio_List[i].AudioDirection != 0)
            {
                Audio_List[i].Audio.volume += (ReturnActualVolume(Audio_List[i]) / Audio_List[i].AudioDirection) * Time.unscaledDeltaTime;

                if (Audio_List[i].AudioDirection < 0)
                {



                    if (Audio_List[i].Audio.volume <= 0)
                    {
                        Audio_List[i].Audio.volume = 0;

                        PauseAudio(Audio_List[i]);
                        Audio_List[i].AudioDirection = 0;
                    }
                }
                else
                {
                    if (Audio_List[i].Audio.volume >= ReturnActualVolume(Audio_List[i]))
                    {
                        Audio_List[i].Audio.volume = ReturnActualVolume(Audio_List[i]);

                        Audio_List[i].AudioDirection = 0;
                    }
                }
            }

            if (Audio_List[i].SpatialTransform)
            {
                Audio_List[i].Audio.transform.position = Audio_List[i].SpatialTransform.position;
            }
        }
    }


    public void RefreshAudioVolumes()
    {
        for (int i = 0; i < Audio_List.Count; i++)
        {
            Audio_List[i].Audio.volume = PlayerPrefs.GetFloat(AudioName) * Audio_List[i].AudioVolume;

        }
    }

    public float ReturnActualVolume(AudioData data)
    {
        return data.AudioVolume * PlayerPrefs.GetFloat(AudioName);
    }

    public bool ReturnActiveAudio(AudioData data)
    {

        if (data.SceneNumber == SceneManager.GetActiveScene().buildIndex && data.Audio.clip && (data.Audio.isPlaying || data.isPaused || data.Audio.time > 0f && data.Audio.time < data.Audio.clip.length))
        {

            return true;
        }
        else
        {
            data.Audio.Stop();
            return false;
        }
    }

    public void DynamicVolumeChange(float RequestedDirection = 1, int SpecificPosition = -1)
    {
        if (AudioName == "Sound")
        {
            //Debug.Log("PAUSING" + SpecificPosition);
        }

        for (int i = 0; i < Audio_List.Count; i++)
        {
            if (SpecificPosition == -1 || i == SpecificPosition)
            {


                if (Audio_List[i].Piority >= 0 && ReturnActiveAudio(Audio_List[i])) //negative means it cant decrease volume
                {
                    if (RequestedDirection > 0)
                    {
                        PauseAudio(Audio_List[i], false);
                    }

                    Audio_List[i].AudioDirection = RequestedDirection;
                }
            }
        }
    }

    void PauseAudio(AudioData audioData, bool Pause = true)
    {
        audioData.isPaused = Pause;

        if (Pause)
        {
            audioData.Audio.Pause();

            if (AudioName == "Sound")
            {
                SetupAudioData(audioData, null);
            }
        }
        else
        {


            if (audioData.Audio.time == 0)
            {
                audioData.Audio.time = 0; //error without this?
                audioData.Audio.Play();
            }
            else
            {
                audioData.Audio.UnPause();
            }
        }
    }
    /*
	 
	playing a song will force all songs to go quiet, then once that song has played. the song playing before will continue

	//[done]
	//[playing]
	//[next]

	//calls temporary song to play

	//[temporary song]
	//[continue playing]
	//[play]

	 */

    AudioData SetupAudioData(AudioData audioData, AudioClip RequestedAudio, bool isLooped = false)
    {
        audioData.SceneNumber = SceneManager.GetActiveScene().buildIndex;
        audioData.Audio.Stop();
        audioData.Audio.clip = RequestedAudio;
        audioData.Audio.volume = 0;
        audioData.Piority = 0;
        audioData.AudioVolume = 1;
        audioData.AudioDirection = 0;
        audioData.Audio.loop = isLooped;
        audioData.TransitionTime = 5;
        audioData.isPaused = false;
        return audioData;
    }


    public int PlaySound(AudioClip RequestedSound, float RequestedVolume = 1, bool ChangePitch = true, int Piority = 0, bool isLooped = false, Transform SpatialTransform = null)
    {
        if (RequestedSound)
        {
            //Piorities:

            //add negative to ignore volume changes
            //0 disposable, can be overwritten by any piority including itself
            //anything above can only be overwritten from something above that level

            bool ArrayIsFull = true;

            for (int i = PoolPosition; i < Audio_List.Count + PoolPosition; i++)
            {
                int CurrentCycle = (int)GameManager.ReturnThresholds(i + 1, Audio_List.Count - 1);

                if (Audio_List[CurrentCycle].Piority == 0 || !ReturnActiveAudio(Audio_List[CurrentCycle]) || Audio_List[CurrentCycle].Piority < Piority)
                {
                    //Debug.Log("after " + PoolPosition);
                    PoolPosition = CurrentCycle;
                    ArrayIsFull = false;
                    break;
                }
            }

            if (ArrayIsFull)
            {
                Debug.LogError("Sound could not be played as the array is full!");
                return 0;
            }

            if (ChangePitch)
            {
                Audio_List[PoolPosition].Audio.pitch = Random.Range(0.98f, 1.02f);
            }
            else
            {
                Audio_List[PoolPosition].Audio.pitch = 1;
            }

            if (SpatialTransform) //default is Vector3.zero
            {
                Audio_List[PoolPosition].Audio.spatialBlend = 1;
                Audio_List[PoolPosition].Audio.transform.position = SpatialTransform.position; //stops the high pitch sound
            }
            else
            {
                Audio_List[PoolPosition].Audio.transform.position = Vector3.zero;
                Audio_List[PoolPosition].Audio.spatialBlend = 0;
            }

            SetupAudioData(Audio_List[PoolPosition], RequestedSound, isLooped);
            Audio_List[PoolPosition].AudioVolume = RequestedVolume;
            Audio_List[PoolPosition].Piority = Piority;
            Audio_List[PoolPosition].SpatialTransform = SpatialTransform;
            Audio_List[PoolPosition].Audio.volume = ReturnActualVolume(Audio_List[PoolPosition]);

            Audio_List[PoolPosition].Audio.Play();
        }

        return PoolPosition;
    }

    //When Music is called, it turns off all other music and plays this one, until finished then returning down the queue

    public void PlayMusic(AudioClip RequestedMusic, float RequestedTransitionTime = 5, bool isLooped = true)
    {
        //transition time is -1 then no songs will be played afterwards for that scene

        if (RequestedMusic)
        {

            DynamicVolumeChange(-1);

            for (int i = 0; i < Audio_List.Count; i++)
            {
                if (ReturnActiveAudio(Audio_List[i]) && Audio_List[i].Audio.clip == RequestedMusic)
                {
                    //just continues playing that music
                    PoolPosition = i;
                    DynamicVolumeChange(0.5f, PoolPosition);
                    return;
                }
            }


            PoolPosition = (int)GameManager.ReturnThresholds(PoolPosition - 1, Audio_List.Count - 1);
            SetupAudioData(Audio_List[PoolPosition], RequestedMusic, isLooped);
            Audio_List[PoolPosition].TransitionTime = RequestedTransitionTime;


            Audio_List[PoolPosition].Audio.Play();
            DynamicVolumeChange(0.5f, PoolPosition);
        }
    }



    public void StopCurrentMusic()
    {

        DynamicVolumeChange(-1, PoolPosition);

    }

    public void StopSelectedSound(AudioClip audioClip)
    {
        for (int i = 0; i < Audio_List.Count; i++)
        {
            if (Audio_List[i].Audio.clip == audioClip)
            {
                DynamicVolumeChange(-1, i);
            }
        }
    }
}
