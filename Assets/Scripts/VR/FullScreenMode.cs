using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

namespace PsychImmersion.VR
{
    public class FullScreenMode : MonoBehaviour
    {

        public Toggle ModeCheckbox;

        private bool _delegateRegistered = false;

        public bool UseTwoDisplays {
            get {
                return ExperimentManager.DualDisplayMode;
            }

            set {
                if (Display.displays.Length == 1) return;
                ExperimentManager.DualDisplayMode = value;
                if (value)
                {
                    Display.displays[1].Activate();
                }
            }
        }

        // Use this for initialization
        void Start () {
            if (!VRSettings.isDeviceActive)
            {
                var resolutions = Screen.resolutions;
                var res = resolutions[resolutions.Length - 1];
                Screen.SetResolution(res.width, res.height, true, res.refreshRate);
                Display.onDisplaysUpdated += OnOnDisplaysUpdated;
                _delegateRegistered = true;
                OnOnDisplaysUpdated();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_delegateRegistered) Display.onDisplaysUpdated -= OnOnDisplaysUpdated;
        }


        private void OnOnDisplaysUpdated()
        {
            if (Display.displays.Length == 1)
            {
                ExperimentManager.DualDisplayMode = false;
                ModeCheckbox.isOn = false;
                ModeCheckbox.interactable = false;
            }
            else
            {
                ModeCheckbox.interactable = true;
            }
        }
    }
}
