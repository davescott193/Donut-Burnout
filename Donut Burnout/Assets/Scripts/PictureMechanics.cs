using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureMechanics : MonoBehaviour
{
    public Camera ActiveCamera;

    public CustomRenderTexture ActiveCustomRenderTexture;

    public Transform ReturnChildTransform()
    {
        return transform.childCount > 1 ? transform.GetChild(1) : null;
    }

    private void Update()
    {
        ReturnChildTransform().Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
    }

    public Texture ReturnPicture(bool realtimeBool = false, float cameraShiftFloat = 1)
    {
        int resolutionInt = 600;

        CustomRenderTexture activeRenderTexture = ActiveCustomRenderTexture ? ActiveCustomRenderTexture : new CustomRenderTexture(resolutionInt, resolutionInt);
        activeRenderTexture.initializationColor = new Color(0, 0, 0, 0);

        ActiveCamera.enabled = true;

        if (realtimeBool)
        {
            activeRenderTexture.updateMode = CustomRenderTextureUpdateMode.Realtime;
            activeRenderTexture.doubleBuffered = true;
        }
        else
        {
            activeRenderTexture.updateMode = CustomRenderTextureUpdateMode.OnDemand;
        }

        CalculateCameraToObject(ActiveCamera, ReturnChildTransform(), cameraShiftFloat);

        ActiveCamera.targetTexture = activeRenderTexture;
        ActiveCamera.Render();

        if (!realtimeBool)
        {
            ActiveCamera.enabled = false;
        }

        ActiveCustomRenderTexture = activeRenderTexture;
        //LoadManager.singleton.GlobalCustomRenderTextureList.Add(ActiveCustomRenderTexture);
        //  Destroy(ActiveCustomRenderTexture, 1);
        return ActiveCamera.targetTexture;
    }

    public Bounds ReturnModelBounds(Transform requestedTransform)
    {
        Bounds activeBounds = new Bounds();
        activeBounds.center = requestedTransform.transform.position;
        activeBounds.size = Vector3.zero;

        for (int i = 0; i < requestedTransform.childCount; i++)
        {
            if (requestedTransform.GetChild(i).GetComponent<MeshRenderer>())
            {
                activeBounds.Encapsulate(requestedTransform.GetChild(i).GetComponent<MeshRenderer>().bounds);
            }
        }

        return activeBounds;
    }

    public void CalculateCameraToObject(Camera RequestedCamera, Transform requestedTransform, float cameraShiftFloat = 1)
    {
        Bounds activeBounds = ReturnModelBounds(requestedTransform);

        Vector3 objectSizes = activeBounds.max - activeBounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * RequestedCamera.fieldOfView); // Visible height 1 meter in front
        float distance = cameraShiftFloat * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        RequestedCamera.transform.position = activeBounds.center - distance * RequestedCamera.transform.forward;
        //Debug.Log(RequestedCamera.transform.forward + " | " + activeBounds.center - distance * RequestedCamera.transform.forward);
    }

    private void OnDestroy()
    {
        if (ActiveCustomRenderTexture && GameManager.instance)
        {
            Destroy(ActiveCustomRenderTexture);
        }
    }
}
