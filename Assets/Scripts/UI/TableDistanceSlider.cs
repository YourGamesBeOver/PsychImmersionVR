using System;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    [RequireComponent(typeof(Slider))]
    public class TableDistanceSlider : MonoBehaviour
    {

        public Text ValueText;
        private Slider _slider;
        private float _curValue;
        private BoxManager _boxManager;
        public int DecimalPlaces = 1;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            _boxManager = FindObjectOfType<BoxManager>();
            if (_boxManager == null) {
                Debug.LogError("Could not find BoxManager");
                _slider.interactable = false;
                if (ValueText != null) ValueText.text = "ERR";
                return;
            }

            _curValue = (float)Math.Round(_boxManager.DistanceFromPlayer, DecimalPlaces);
            _slider.value = _curValue;
            UpdateText();
        }

        private void OnValueChanged(float newValue)
        {
            var rounded = (float)Math.Round(newValue, DecimalPlaces);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_curValue == rounded) return;
            _curValue = rounded;
            _slider.value = rounded;
            _boxManager.DistanceFromPlayer = rounded;
            UpdateText();
        }

        private void UpdateText()
        {
            if (ValueText != null) ValueText.text = string.Format("{0:F1}m", _curValue);
        }

    }
}
