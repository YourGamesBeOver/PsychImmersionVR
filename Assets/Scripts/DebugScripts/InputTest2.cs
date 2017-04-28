using PsychImmersion.CrossPlatformInput;
using UnityEngine;

namespace PsychImmersion.DebugScripts
{
    public class InputTest2 : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            Debug.Log(string.Join(", ", Input.GetJoystickNames()));
            CrossPlatformInputManager.Instance.ConfirmButtonPressed += ConfirmButtonPressed;
            CrossPlatformInputManager.Instance.AbortButtonPressed += AbortButtonPressed;
            CrossPlatformInputManager.Instance.DownButtonPressed += DownButtonPressed;
            CrossPlatformInputManager.Instance.UpButtonPressed += UpButtonPressed;
        }

        private void OnDestroy()
        {
            if (CrossPlatformInputManager.Instance == null) return;
            CrossPlatformInputManager.Instance.ConfirmButtonPressed -= ConfirmButtonPressed;
            CrossPlatformInputManager.Instance.AbortButtonPressed -= AbortButtonPressed;
            CrossPlatformInputManager.Instance.DownButtonPressed -= DownButtonPressed;
            CrossPlatformInputManager.Instance.UpButtonPressed -= UpButtonPressed;
        }

        private void UpButtonPressed() {
            Debug.Log("Up Pressed!");
        }

        private void DownButtonPressed() {
            Debug.Log("Down Pressed!");
        }

        private void AbortButtonPressed() {
            Debug.Log("Back Pressed!");
        }

        private void ConfirmButtonPressed() {
            Debug.Log("Next Pressed!");
        }
    }
}
