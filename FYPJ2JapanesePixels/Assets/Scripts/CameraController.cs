using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CamSettings
{
    public float yOffset;
    public enum CamMode
    {
        E_PCENTERED,
        E_OFFSET,
        E_WHOLEMAP
    }
    public CamMode mode;
}

public class CameraController : MonoBehaviour 
{
    public GameObject zoomTarget;
    private CamSettings settings;
    Vector3 pos;

	// Use this for initialization
	void Start () {
        settings.yOffset = Camera.main.orthographicSize;
        settings.mode = CamSettings.CamMode.E_WHOLEMAP;
	}
	
	// Update is called once per frame
	void Update () 
    {
        zoomTarget = MyNetwork.instance.localPlayer;

        if (zoomTarget != null)
        {
            if (settings.mode == CamSettings.CamMode.E_OFFSET)
                pos = new Vector3(zoomTarget.transform.position.x, zoomTarget.transform.position.y - settings.yOffset * 0.5f, transform.position.z);
            else if (settings.mode == CamSettings.CamMode.E_PCENTERED)
                pos = new Vector3(zoomTarget.transform.position.x, zoomTarget.transform.position.y, transform.position.z);
        }

        if (settings.mode != CamSettings.CamMode.E_WHOLEMAP)
            transform.position = pos;
	}

    public void switchMode(CamSettings.CamMode cameraMode)
    {
        settings.mode = cameraMode;
    }
}
