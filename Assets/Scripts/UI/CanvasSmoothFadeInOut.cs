using System;
using System.Collections;
using UnityEngine;

namespace PsychImmersion.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasSmoothFadeInOut : MonoBehaviour
    {
        public float Duration = 2f;
        public bool DisableWhenInvisible = true;
        public bool InitiallyVisible = true;
        public bool FadeInOnEnable = true;

        private CanvasGroup _group;
        private float _targetAlpha = 1;

        private event Action OnFadeIn;
        private event Action OnFadeOut;


        void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _targetAlpha = _group.alpha;
        }

        private void OnEnable()
        {
            _group.alpha = InitiallyVisible ? _targetAlpha : 0f;
            if(FadeInOnEnable) StartCoroutine(FadeInCorutine());
        }

        private void Start()
        {
            if (!InitiallyVisible && DisableWhenInvisible) gameObject.SetActive(false);
        }

        public void FadeOut(Action callback)
        {
            if (!gameObject.activeInHierarchy)
            {
                if (callback != null) callback();
                return;
            }
            if(callback != null) OnFadeOut += callback;
            StopAllCoroutines();
            StartCoroutine(FadeOutCorutine());
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeIn(Action callback)
        {
            if (!gameObject.activeInHierarchy && DisableWhenInvisible) gameObject.SetActive(true);
            if (callback != null) OnFadeIn += callback;
            StopAllCoroutines();
            StartCoroutine(FadeInCorutine());
        }

        public void FadeIn()
        {
            FadeIn(null);
        }

        private IEnumerator FadeInCorutine()
        {
            float time = 0;
            while (time < Duration)
            {
                _group.alpha = Mathf.Lerp(0f, _targetAlpha, time/Duration);
                yield return null;
                time += Time.deltaTime;
            }
            _group.alpha = _targetAlpha;
            if (OnFadeIn != null)
            {
                OnFadeIn();
                OnFadeIn = null;
            }
        }

        private IEnumerator FadeOutCorutine() {
            float time = 0;
            while (time < Duration) {
                _group.alpha = Mathf.Lerp(_targetAlpha, 0f, time / Duration);
                yield return null;
                time += Time.deltaTime;
            }
            _group.alpha = 0f;
            if(DisableWhenInvisible)gameObject.SetActive(false);
            if (OnFadeOut != null) {
                OnFadeOut();
                OnFadeOut = null;
            }
        }
    }
}
