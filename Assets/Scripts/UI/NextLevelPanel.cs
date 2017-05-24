using System;
using PsychImmersion.CrossPlatformInput;
using UnityEngine;

namespace PsychImmersion.UI
{
    public class NextLevelPanel : MonoBehaviour
    {
        public CanvasSmoothFadeInOut Fader;

        private bool _visible = false;
        public event Action OnConfirm;

        private void Start()
        {
            CrossPlatformInputManager.Instance.NextLevelButtonPressed += OnNextLevelButton;
        }

        private void OnDestroy()
        {
            if (CrossPlatformInputManager.Instance != null)
            {
                CrossPlatformInputManager.Instance.NextLevelButtonPressed -= OnNextLevelButton;
            }
        }

        private void OnNextLevelButton()
        {
            if (!_visible) return;
            Debug.Log("Next Level prompt confirmed!");
            if (OnConfirm != null) OnConfirm();
            _visible = false;
            Fader.FadeOut();
        }

        public void Prompt()
        {
            if (_visible) return;
            Fader.FadeIn();
            _visible = true;
        }
    }
}
