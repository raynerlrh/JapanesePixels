using UnityEngine;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour 
{
    [SyncVar]
    Vector3 realPosition = Vector3.zero;

    [SyncVar]
    Quaternion realRotation;

    public bool b_isEnemy { get; set; }
    private float updateInterval;

    void Update()
    {
        if (isLocalPlayer || b_isEnemy)
            UpdateSync();
        else
            Interpolate();
    }

    // Call under isLocalPlayer
    void UpdateSync()
    {
        // Update the server with position/rotation
        updateInterval += Time.deltaTime;
        if (updateInterval > 0.11f) // 9 times per sec (default unity send rate)
        {
            updateInterval = 0;
            CmdSync(transform.position, transform.rotation);
        }
    }

    void Interpolate()
    {
        transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
        //transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation)
    {
        realPosition = position;
        realRotation = rotation;
    }
}
