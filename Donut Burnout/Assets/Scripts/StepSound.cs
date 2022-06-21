using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    public void PlaySoundVoid()
    {
        GameManager.instance.SoundPool.PlaySound(GameManager.instance.PlayerFootStepSound, 1, true, 0, false, transform);
    }

}
