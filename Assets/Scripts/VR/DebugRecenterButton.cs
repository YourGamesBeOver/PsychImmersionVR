using UnityEngine;
using UnityEngine.VR;

namespace PsychImmersion.VR
{
    public class DebugRecenterButton : MonoBehaviour
    {
        public KeyCode ToggleKey = KeyCode.R;

        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(ToggleKey))
            {
                InputTracking.Recenter();
            }
        }
    }
}
