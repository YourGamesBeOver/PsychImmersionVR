using PsychImmersion.CrossPlatformInput;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace PsychImmersion.UI
{
    public class AbortPanel : MonoBehaviour
    {

        public UICircle Circle;
        public float HoldTime = 5f;

        public CanvasSmoothFadeInOut Fader;

        private float _curHoldTime = 0f;



        // Use this for initialization
        void Start () {
		    CrossPlatformInputManager.Instance.AbortButtonPressed += OnAbortButtonPressed;
        }

        private void OnDestroy()
        {
            if (CrossPlatformInputManager.Instance != null)
            {
                CrossPlatformInputManager.Instance.AbortButtonPressed -= OnAbortButtonPressed;
            }
        }

        private void OnAbortButtonPressed() {
            enabled = true;
            Fader.FadeIn();
            _curHoldTime = 0f;
        }

        // Update is called once per frame
        void Update () {
            if (CrossPlatformInputManager.Instance.AbortButtonDown)
            {
                _curHoldTime += Time.deltaTime;
                if (_curHoldTime > HoldTime)
                {
                    ExperimentManager.Instance.AbortExperiment();
                }
                else
                {
                    Circle.fillPercent = (int)Mathf.Round((_curHoldTime / HoldTime) * 100);
                    Circle.SetVerticesDirty();
                }
            }
            else
            {
                Fader.FadeOut();
                enabled = false;
            }
        }
    }
}
