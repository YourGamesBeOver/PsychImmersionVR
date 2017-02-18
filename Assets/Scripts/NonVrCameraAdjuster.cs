using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// This script adjusts the camera for when we are not using VR
/// </summary>
public class NonVrCameraAdjuster : MonoBehaviour
{

    public float CameraHeight;

	// Use this for initialization
	void Start () {
	    if (VRSettings.isDeviceActive)
	    {
	        enabled = false;
	    }
	    else
	    {
            var pos = transform.localPosition;
            pos.y = CameraHeight;
	        transform.localPosition = pos;
	    }
    }
}
