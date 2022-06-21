using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
    public Text PromptText;
    public RawImage PromptRawImage;
    public MechanicsManager.CustomerData CustomerData;
    private void Update()
    {

        transform.LookAt(MechanicsManager.instance.characterMotors[0].m_look.transform);
    }

    public void CreatePicture()
    {
        PictureMechanics activePictureMechanics = Instantiate(MechanicsManager.instance.PictureMechanicsPrefab, MechanicsManager.instance.PictureHolderTransform).GetComponent<PictureMechanics>();
        activePictureMechanics.transform.localPosition = new Vector3(0, MechanicsManager.instance.PictureHolderTransform.childCount * 10, 0);
        Transform foodTransform = Instantiate(GameManager.instance.FoodList[CustomerData.FoodTypeInt], activePictureMechanics.transform).transform;
        foodTransform.localPosition = Vector3.zero;
        PromptRawImage.texture = activePictureMechanics.ReturnPicture(true);
    }
}
