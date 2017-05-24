using System;
using PsychImmersion.CrossPlatformInput;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class NextLevelPanel : MonoBehaviour
    {
        public CanvasSmoothFadeInOut Fader;
        public Text Text;

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
            if (Text != null) Text.text = GetText();
            Fader.FadeIn();
            _visible = true;
        }

        private string GetText()
        {
            return StringManager.GetString("NextLevelPanel_MessagePrefix") +
                   GetDescriptionForDifficulity(DifficultyManager.Instance.CurrentDifficulty + 1);
        }

        private static string GetDescriptionForDifficulity(Difficulity dif)
        {
            switch (dif)
            {
                case Difficulity.Beginner:
                    return StringManager.GetString("NextLevelPanel_Beginner1Description");
                case Difficulity.Beginner2:
                    return StringManager.GetString("NextLevelPanel_Beginner2Description");
                case Difficulity.Beginner3:
                    return StringManager.GetString("NextLevelPanel_Beginner3Description");
                case Difficulity.Intermediate:
                    return StringManager.GetString("NextLevelPanel_Intermediate1Description");
                case Difficulity.Intermediate2:
                    return StringManager.GetString("NextLevelPanel_Intermediate2Description");
                case Difficulity.Intermediate3:
                    return StringManager.GetString("NextLevelPanel_Intermediate3Description");
                case Difficulity.Advanced:
                    return StringManager.GetString("NextLevelPanel_Advanced1Description");
                case Difficulity.Advanced2:
                    return StringManager.GetString("NextLevelPanel_Advanced2Description");
                case Difficulity.Advanced3:
                    return StringManager.GetString("NextLevelPanel_Advanced3Description");
                case Difficulity.End:
                    return StringManager.GetString("NextLevelPanel_EndDescription");
                default:
                    return "[ERR: No Description available]";
            }
        }
    }
}
