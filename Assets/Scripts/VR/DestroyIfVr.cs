using UnityEngine;
using UnityEngine.VR;

namespace PsychImmersion.VR
{
    public class DestroyIfVr : MonoBehaviour {

        public Mode DestroyMode = Mode.DestroyIfVr;

        public enum Mode
        {
            DestroyIfVr,
            DestroyIfNotVr
        }

        // Use this for initialization
        void Start () {
            if (VRSettings.isDeviceActive && DestroyMode == Mode.DestroyIfVr)
            {
                Destroy(this.gameObject);
            }
            if (!VRSettings.isDeviceActive && DestroyMode == Mode.DestroyIfNotVr)
            {
                Destroy(this.gameObject);
            }
            this.enabled = false;
        }
	
        // Update is called once per frame
        void Update () {
		
        }
    }
}
