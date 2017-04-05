using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PsychImmersion.UI
{
    public class AsyncSceneLoader : MonoBehaviour
    {

        public string SceneToLoad;
        public Slider ProgressBar;

        private AsyncOperation _asyncOperation;

        // Use this for initialization
        void Start ()
        {
            _asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad);
        }
	
        // Update is called once per frame
        void Update ()
        {
            ProgressBar.value = _asyncOperation.progress;
        }
    }
}
