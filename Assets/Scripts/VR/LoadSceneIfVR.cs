using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

namespace PsychImmersion.VR
{
    public class LoadSceneIfVR : MonoBehaviour
    {
        public string[] ScenesToLoad;

        private void Start()
        {
            if (!VRSettings.isDeviceActive) return;
            foreach (var scene in ScenesToLoad)
            {
                if (!SceneManager.GetSceneByName(scene).isLoaded)
                    SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
            enabled = false;
        }
    }
}
