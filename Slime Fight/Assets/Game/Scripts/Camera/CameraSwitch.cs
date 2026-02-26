using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCam;
    public Camera thirdPersonCamFront;
    public Camera thirdPersonCamBack;

    private Camera[] cams;
    private int currentIndex;

    void Start()
    {
        var list = new List<Camera> { firstPersonCam, thirdPersonCamBack, thirdPersonCamFront };

        cams = list.ToArray();
        currentIndex = 0;
        ApplyActiveCamera();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (cams == null || cams.Length == 0)
                return;

            currentIndex = (currentIndex + 1) % cams.Length;
            ApplyActiveCamera();
        }
    }

    private void ApplyActiveCamera()
    {
        if (cams == null)
            return;

        for (int i = 0; i < cams.Length; i++)
        {
            if (cams[i] != null)
                cams[i].enabled = (i == currentIndex);
        }
    }
}