using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
    public Text PromptText;
    public Image PromptImage;

    private void Update()
    {
        transform.LookAt(MechanicsManager.instance.characterMotors[0].m_look.transform);
    }
}
