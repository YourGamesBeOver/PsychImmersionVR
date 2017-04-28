﻿using PsychImmersion.CrossPlatformInput;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class StressSelectorPanel : MonoBehaviour
    {
        private const int MaxStressLevel = 20;

        public Text NumberText;
        public GameObject UpArrow;
        public GameObject DownArrow;
        public CanvasSmoothFadeInOut Fader;

        public delegate bool SubmitCallback(int value);

        private SubmitCallback _submitCallback = null;

        private int _curLevel = 5;
        //this value is false when accepting user input
        private bool _submitted = true;


        public void SetStressLevel(int newLevel)
        {
            _curLevel = newLevel;
            UpArrow.SetActive(newLevel < MaxStressLevel);
            DownArrow.SetActive(newLevel > 0);
            var half = newLevel/2f;
            NumberText.text = half.ToString("F1");
        }

        public void Increment()
        {
            if (_submitted) return;
            if (_curLevel < MaxStressLevel) SetStressLevel(_curLevel+1);
        }

        public void Decrement()
        {
            if (_submitted) return;
            if (_curLevel > 0) SetStressLevel(_curLevel-1);
        }

        public void Submit()
        {
            if (_submitted) return; //don't allow multiple submissions
            //submit _curLevel to data storage object
            if (_submitCallback == null || _submitCallback.Invoke(_curLevel))
            {
                DataRecorder.RecordEvent(DataEvent.AnxietyLevel, _curLevel);
            }
            _submitted = true;
            CancelInvoke("Submit");
            Fader.FadeOut();
        }

        public int CurrentStressLevel { get { return _curLevel; } }

        public void Prompt(float timeout, SubmitCallback callback = null)
        {
            _submitCallback = callback;
            Fader.FadeIn();
            if(!float.IsPositiveInfinity(timeout)) Invoke("Submit", timeout);
            _submitted = false;
        }


        // Use this for initialization
        void Start ()
        {
            CrossPlatformInputManager.Instance.UpButtonPressed += Increment;
            CrossPlatformInputManager.Instance.DownButtonPressed += Decrement;
            CrossPlatformInputManager.Instance.ConfirmButtonPressed += Submit;
            SetStressLevel(_curLevel);
            Fader.FadeOut();
        }

        private void OnDestroy()
        {
            if (CrossPlatformInputManager.Instance == null) return;
            CrossPlatformInputManager.Instance.UpButtonPressed -= Increment;
            CrossPlatformInputManager.Instance.DownButtonPressed -= Decrement;
            CrossPlatformInputManager.Instance.ConfirmButtonPressed -= Submit;
        }

#if DEBUG

        // Update is called once per frame
        void Update () {
            //debug
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Decrement();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Increment();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Submit();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Prompt(30f);
            }

        }
#endif
    }
}
