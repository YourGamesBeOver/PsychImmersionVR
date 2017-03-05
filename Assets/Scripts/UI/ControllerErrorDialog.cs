using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class ControllerErrorDialog : MonoBehaviour
    {

        public CanvasSmoothFadeInOut Fader;
        public Text ErrorText;

        public void OnError(string text)
        {
            ErrorText.text = text;
            Fader.FadeIn();
        }

        public void OnErrorCleared()
        {
            Fader.FadeOut();
        }

    }
}
