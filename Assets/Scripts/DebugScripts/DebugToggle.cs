using System;
using UnityEngine;
using UnityEngine.Events;

namespace DebugScripts
{
    public class DebugToggle : MonoBehaviour
    {

        private static DebugToggleMaster _master = null;

        public UnityEvent DisableOverride;
        public UnityEvent EnableOveride;

        void Awake()
        {
            if (_master == null)
            {
                var go = new GameObject("DebugToggleMaster", typeof(DebugToggleMaster));
                DontDestroyOnLoad(go);
                _master = go.GetComponent<DebugToggleMaster>();
            }

            _master.OnDebugModeChange += DebugModeChanged;
            DebugModeChanged(_master.DebugMode);
        }


        private void DebugModeChanged(bool mode)
        {
            if (!mode && DisableOverride.GetPersistentEventCount() > 0)
            {
                DisableOverride.Invoke();
            }
            else if (mode && EnableOveride.GetPersistentEventCount() > 0)
            {
                EnableOveride.Invoke();
            }
            else
            {
                gameObject.SetActive(mode);
            }
        }

        private void OnDestroy()
        {
            _master.OnDebugModeChange -= DebugModeChanged;
        }
    }

    public class DebugToggleMaster : MonoBehaviour {

        public event Action<bool> OnDebugModeChange;
        [System.NonSerialized]
        public bool DebugMode = DefaultMode;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.BackQuote)) {
                DebugMode = !DebugMode;
                if (OnDebugModeChange != null) OnDebugModeChange(DebugMode);
            }
        }
#if UNITY_EDITOR || DEBUG
        private const bool DefaultMode = true;
#else
    private const bool DefaultMode = false;
#endif
    }
}