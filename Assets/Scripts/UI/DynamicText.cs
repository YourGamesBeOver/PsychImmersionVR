using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    /// <summary>
    /// This class will set the value of a Unity Canvas Text to a value loaded from the StringManager
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class DynamicText : MonoBehaviour
    {

        public string DynamicTextKey;

        // Use this for initialization
        void Start ()
        {
            GetComponent<Text>().text = StringManager.GetString(DynamicTextKey);
        }
	
    }
}
