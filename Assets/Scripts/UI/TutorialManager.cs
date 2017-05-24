using System;
using System.Collections;
using PsychImmersion.CrossPlatformInput;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

namespace PsychImmersion.UI
{
    public class TutorialManager : DifficultySensitiveBehaviour
    {
        public Text TutorialText;
        public CanvasSmoothFadeInOut TutorialPanelFader;
        public XboxControllerRenderer ControllerRenderer;
        public CanvasSmoothFadeInOut ControllerPanelFader;
        public AutoLerp ControllerPanelLerp;
        public Transform AlternativeLerpPoint;
        public StressSelectorPanel StressPanel;

        private Stage _curStage = Stage.Welcome;

        private bool _sawUpDown = false;

        // Use this for initialization
        void Start () {
            ControllerRenderer.Hide(XboxControllerRenderer.XboxButton.All);
		    GoToStage(Stage.Welcome);
            CrossPlatformInputManager.Instance.UpButtonPressed += OnUpDownPressed;
            CrossPlatformInputManager.Instance.DownButtonPressed += OnUpDownPressed;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (CrossPlatformInputManager.Instance != null)
            {
                CrossPlatformInputManager.Instance.UpButtonPressed -= OnUpDownPressed;
                CrossPlatformInputManager.Instance.DownButtonPressed -= OnUpDownPressed;
            }
        }

        public override void SetLevel(Difficulity level)
        {
            if (level != Difficulity.Tutorial)
            {
                TutorialPanelFader.FadeOut(() =>
                {
                    this.gameObject.SetActive(false);
                });
                ControllerPanelFader.FadeOut(() => ControllerPanelFader.gameObject.SetActive(false));
            }
        }

        private void OnUpDownPressed()
        {
            if (_curStage != Stage.StressUpDown || _sawUpDown) return;
            GoToStageAfter(Stage.StressConfirm, 10f);
            _sawUpDown = true;
        }

        private IEnumerator GoToStageCoroutine(Stage stage, float delay)
        {
            yield return new WaitForSeconds(delay);
            GoToStage(stage);
        }

        private void GoToStageAfter(Stage stage, float delay)
        {
            StartCoroutine(GoToStageCoroutine(stage, delay));
        }

        private void GoToStage(Stage stage)
        {
            _curStage = stage;
            SetTextForStage(stage);
            switch (stage)
            {
                case Stage.Welcome:
                    GoToStageAfter(Stage.Abort, 10f);
                    break;
                case Stage.Abort:
                    ControllerRenderer.Blink(XboxControllerRenderer.XboxButton.LT | XboxControllerRenderer.XboxButton.RT | XboxControllerRenderer.XboxButton.B);
                    ControllerPanelFader.FadeIn();
                    GoToStageAfter(VRSettings.isDeviceActive ? Stage.StressPre : Stage.Look, 10f);
                    break;
                case Stage.Look:
                    ControllerRenderer.Blink(XboxControllerRenderer.XboxButton.RS);
                    GoToStageAfter(Stage.StressPre, 10f);
                    break;
                case Stage.StressPre:
                    ControllerPanelFader.FadeOut();
                    GoToStageAfter(Stage.StressUpDown, 15f);
                    break;
                case Stage.StressUpDown:
                    ControllerPanelFader.FadeIn();
                    ControllerRenderer.Blink(XboxControllerRenderer.XboxButton.LS | XboxControllerRenderer.XboxButton.Dpad);
                    StressPanel.SubmissionEnabled = false;
                    PromptForBaseline();
                    break;
                case Stage.StressConfirm:
                    ControllerRenderer.Blink(XboxControllerRenderer.XboxButton.A);
                    StressPanel.SubmissionEnabled = true;
                    break;
                case Stage.MovingOn:
                    //we need to move the controller over to the left side
                    ControllerPanelFader.FadeOut(() =>
                    {
                        ControllerPanelLerp.Target = AlternativeLerpPoint;
                        ControllerPanelFader.FadeIn();
                        ControllerRenderer.Blink(XboxControllerRenderer.XboxButton.X);
                        DifficultyManager.Instance.PromptForNextDifficultyLevel();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage", stage, null);
            }
        }


        private void SetTextForStage(Stage stage)
        {
            TutorialPanelFader.FadeOut(() =>
            {
                TutorialText.text = GetTextForStage(stage);
                TutorialPanelFader.FadeIn();
            });
            
        }

        private string GetTextForStage(Stage stage)
        {
            switch (stage)
            {
                case Stage.Welcome:
                    return StringManager.GetString("Tutorial_WelcomeText");
                case Stage.Abort:
                    return StringManager.GetString("Tutorial_AbortText");
                case Stage.Look:
                    return StringManager.GetString("Tutorial_LookText");
                case Stage.StressPre:
                    return StringManager.GetString("Tutorial_StressIntroText");
                case Stage.StressUpDown:
                    return StringManager.GetString("Tutorial_StressUpDownText");
                case Stage.StressConfirm:
                    return StringManager.GetString("Tutorial_StressConfirmText");
                case Stage.MovingOn:
                    return StringManager.GetString("Tutorial_NextLevelText");
                default:
                    throw new ArgumentOutOfRangeException("stage", stage, null);
            }
        }

        private void PromptForBaseline() {
            StressPanel.Prompt(float.PositiveInfinity, value => {
                DataRecorder.RecordEvent(DataEvent.BaselineAnxietyLevel, value);
                GoToStage(Stage.MovingOn);
                DifficultyManager.Instance.SetBaseline(value);
                return false;
            });
        }

#if DEBUG
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StopAllCoroutines();
                GoToStage(Stage.MovingOn);
            }
        }

#endif



        private enum Stage
        {
            Welcome,
            Abort,
            Look,
            StressPre,
            StressUpDown,
            StressConfirm,
            MovingOn
        }
    }
}
