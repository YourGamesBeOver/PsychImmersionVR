﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

namespace PsychImmersion.UI
{
    [RequireComponent(typeof(Slider))]
    public class ResolutionScalingSlider : MonoBehaviour
    {

        public Text ValueText;
        private Slider _slider;
        private float _curValue;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            if (!VRSettings.isDeviceActive)
            {
                _slider.interactable = false;
                ValueText.text = "NoVR";
                return;
            }
            _curValue = (float)Math.Round(VRSettings.renderScale, 1);
            _slider.value = _curValue;
            if (ValueText != null) ValueText.text = "" + _curValue;
        }

        private void OnValueChanged(float newValue)
        {
            var rounded = (float)Math.Round(newValue, 1);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_curValue == rounded) return;
            _curValue = rounded;
            _slider.value = rounded;
            VRSettings.renderScale = rounded;
            if (ValueText != null) ValueText.text = ""+rounded;
        }

    }
}
