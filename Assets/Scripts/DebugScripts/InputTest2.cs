using UnityEngine;
using CrossPlatformInput;

public class InputTest2 : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    Debug.Log(string.Join(", ", Input.GetJoystickNames()));
        CrossPlatformInputManager.Instance.NextButtonPressed += NextButtonPressed;
        CrossPlatformInputManager.Instance.BackButtonPressed += BackButtonPressed;
        CrossPlatformInputManager.Instance.DownButtonPressed += DownButtonPressed;
        CrossPlatformInputManager.Instance.UpButtonPressed += UpButtonPressed;
	}

    private void OnDestroy()
    {
        CrossPlatformInputManager.Instance.NextButtonPressed -= NextButtonPressed;
        CrossPlatformInputManager.Instance.BackButtonPressed -= BackButtonPressed;
        CrossPlatformInputManager.Instance.DownButtonPressed -= DownButtonPressed;
        CrossPlatformInputManager.Instance.UpButtonPressed -= UpButtonPressed;
    }

    private void UpButtonPressed() {
        Debug.Log("Up Pressed!");
    }

    private void DownButtonPressed() {
        Debug.Log("Down Pressed!");
    }

    private void BackButtonPressed() {
        Debug.Log("Back Pressed!");
    }

    private void NextButtonPressed() {
        Debug.Log("Next Pressed!");
    }
}
