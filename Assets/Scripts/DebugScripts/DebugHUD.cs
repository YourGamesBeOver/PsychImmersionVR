using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

//this version of DebugHUD was last updated 4/2/2017
//this version of DebugHUD is modified to work on a canvas
// DebugHUD is an asset by Steven Miller created prior to this project and used with permission
namespace PsychImmersion.DebugScripts
{
    [RequireComponent(typeof(Text))]
    public class DebugHUD : MonoBehaviour {
        private static readonly Dictionary<string, object> List;

        private Text _text;

        public KeyCode ToggleKey = KeyCode.BackQuote;

        static DebugHUD() {
            List = new Dictionary<string, object>();
        }

        public static void SetValue(string key, object value) {
            if (value is bool) {
                SetValue(key, (bool)(value) ? "<color=green>True</color>" : "<color=red>False</color>");
            } else if (value == null) {
                SetValue(key, "<color=red>null</color>");
            } else {
                List[key] = value;
            }
        }

        public static void RemoveKey(string key) {
            List.Remove(key);
        }

        void Start() {
            _text = GetComponent<Text>();
            _text.enabled = Application.isEditor || Debug.isDebugBuild;
        }

        void LateUpdate() {
            if (Input.GetKeyDown(ToggleKey)) {
                _text.enabled = !_text.enabled;
            }
            var s = new StringBuilder();
            foreach (var pair in List) {
                s.AppendFormat("<color=blue>{0}</color>: {1}\n", pair.Key, pair.Value);
            }
            _text.text = s.ToString();
        }
    }
}
