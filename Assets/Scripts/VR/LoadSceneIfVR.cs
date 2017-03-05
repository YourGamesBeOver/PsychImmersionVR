using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

namespace PsychImmersion.VR
{
    public class LoadSceneIfVR : MonoBehaviour
    {

        public SceneAsset[] ScenesToLoad;

        private void Start()
        {
            if (!VRSettings.isDeviceActive) return;
            foreach (var scene in ScenesToLoad)
            {
                if (!SceneManager.GetSceneByName(scene.name).isLoaded)
                    SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
            }
        }
    }
}
