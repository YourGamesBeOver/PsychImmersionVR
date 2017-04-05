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

        private CanvasGroup _group;
        private float _targetAlpha = 1;

        void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _targetAlpha = _group.alpha;
        }

        private void OnEnable()
        {
            _group.alpha = 0;
            if(InitiallyVisible) StartCoroutine(FadeInCorutine());
            if(!InitiallyVisible && DisableWhenInvisible) gameObject.SetActive(true);
        }

        public void FadeOut()
        {
            if (!gameObject.activeInHierarchy) return;
            StopAllCoroutines();
            StartCoroutine(FadeOutCorutine());
        }

        public void FadeIn()
        {
            if (!gameObject.activeInHierarchy && DisableWhenInvisible) gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeInCorutine());
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
        }
    }
}
