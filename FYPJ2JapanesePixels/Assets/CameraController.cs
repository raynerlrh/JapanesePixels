using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject zoomTarget;
    private float yOffset;
	// Use this for initialization
	void Start () {
        yOffset = Camera.main.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = new Vector3(zoomTarget.transform.position.x, zoomTarget.transform.position.y - yOffset * 0.5f, transform.position.z);
        transform.position = pos;
	}
}
