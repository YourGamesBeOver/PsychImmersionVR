using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasSmoothFadeInOut : MonoBehaviour
{
    public float Duration = 2f;

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
        StartCoroutine(FadeInCorutine());
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCorutine());
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
        gameObject.SetActive(false);
    }
}
