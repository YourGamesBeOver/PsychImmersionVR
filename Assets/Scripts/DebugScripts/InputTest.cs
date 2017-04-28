using System.Text;
using PsychImmersion.CrossPlatformInput;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

namespace PsychImmersion.DebugScripts
{
    [RequireComponent(typeof(Text))]
    public class InputTest : MonoBehaviour {

        private Text _text;

        private void Awake() {
            _text = GetComponent<Text>();
        }
	
        // Update is called once per frame
        void Update () {
            if(CrossPlatformInputManager.Instance == null)
            {
                _text.text = "<color=red>CrossPlatformInputManager is null</color>";
                return;
            }
            var sb = new StringBuilder();

            sb.AppendLine("<color=green>Joysticks:</color>");
            var joysticks = Input.GetJoystickNames();
            for(var i = 0; i < joysticks.Length; i++) {
                sb.AppendFormat("<color=blue>{0}</color>: {1}\n", i, joysticks[i]);
            }

            sb.AppendLine("<color=green>Input:</color>");
            sb.AppendLine("<color=blue>Up</color>: " + CrossPlatformInputManager.Instance.UpButtonDown);
            sb.AppendLine("<color=blue>Down</color>: " + CrossPlatformInputManager.Instance.DownButtonDown);
            sb.AppendLine("<color=blue>Confirm</color>: " + CrossPlatformInputManager.Instance.ConfirmButtonDown);
            sb.AppendLine("<color=blue>NextLevel</color>: " + CrossPlatformInputManager.Instance.NextLevelButtonDown);
            sb.AppendLine("<color=blue>Abort</color>: " + CrossPlatformInputManager.Instance.AbortButtonDown);

            sb.AppendLine("<color=green>VRSettings:</color>");
            sb.AppendLine("<color=blue>isDeviceActive</color>: " + VRSettings.isDeviceActive);
            sb.AppendLine("<color=blue>loadedDeviceName</color>: " + VRSettings.loadedDeviceName);

            _text.text = sb.ToString();
        }
    }
}
