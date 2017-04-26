using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class ExitPanel : MonoBehaviour
    {

        public Text FilePathText;

        void Start()
        {
            try
            {
                FilePathText.text = DataRecorder.WriteFile();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e,this);
                FilePathText.text = "An error occured while saving the file.";
            }
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#else
        Application.Quit();
#endif
        }
    }
}
