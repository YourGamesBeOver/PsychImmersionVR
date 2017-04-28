using System;
using System.Linq;
using PsychImmersion.Experiment;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VR;

namespace PsychImmersion.UI {
    /// <summary>
    /// This class loads several scenes asynchronously.  here are the order of events:
    /// 
    /// 1. (on Start()) the required scene list is computed and SceneManager.LoadSceneAsync is called for each of them
    ///     1a. the loaded scenes are told not to activate when they are done loading
    ///     1b. The scenes are all loaded additive except for the first one; this will properly unload the current scene when finished
    /// 2. in Update(), we check if all the scenes are loaded to 90% (this is how unity tells us they are loaded and awaiting activation)
    ///     if not every scene is ready for activation, repeat step 2.  we also update the progressbar in this step
    /// 5. Tell the new scenes to finish loading (set the flag to allow them to activate)
    ///     This will automatically destroy the contents of this scene
    /// 
    /// </summary>
    public class AsyncSceneLoader : MonoBehaviour {
        public LoadableScene[] Scenes;

        public Slider ProgressBar;

        [Serializable]
        public struct LoadableScene {
            // ReSharper disable once UnassignedField.Global
            public string SceneName;
            // ReSharper disable once UnassignedField.Global
            public SceneLoadCondition LoadCondition;
        }

        public enum SceneLoadCondition {
            AlwaysLoad,
            LoadIfVr,
            LoadIfNotVr,
            LoadIfDualDisplay
        }

        private string[] _scenesToLoad;
        private AsyncOperation[] _asyncOperations;

        // Use this for initialization
        void Start() {
            DontDestroyOnLoad(this.gameObject);
            //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            _scenesToLoad =
                Scenes.Where(
                        x =>
                            (x.LoadCondition == SceneLoadCondition.AlwaysLoad) ||
                            (x.LoadCondition == SceneLoadCondition.LoadIfVr && VRSettings.isDeviceActive) ||
                            (x.LoadCondition == SceneLoadCondition.LoadIfNotVr && !VRSettings.isDeviceActive) ||
                            (x.LoadCondition == SceneLoadCondition.LoadIfDualDisplay && (ExperimentManager.DualDisplayMode || VRSettings.isDeviceActive)))
                    .Select(x => x.SceneName).ToArray();

            if (_scenesToLoad.Length == 0) {
                Destroy(this.gameObject);
                return;
            }

            _asyncOperations = new AsyncOperation[_scenesToLoad.Length];
            for (var i = 0; i < _scenesToLoad.Length; i++) {
                _asyncOperations[i] = SceneManager.LoadSceneAsync(_scenesToLoad[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                _asyncOperations[i].allowSceneActivation = i == 0;
            }

        }

        //private void OnDestroy()
        //{
        //    //SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        //}

        //private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        //{
        //    _scenesLoaded++;
        //    if (_scenesLoaded == _scenesToLoad.Length)
        //    {
        //        Destroy(this.gameObject);
        //    }
        //}

        // Update is called once per frame
        void Update() {
            var sum = 0f;
            var readyForActivation = 0;
            foreach (var op in _asyncOperations) {
                sum += op.progress;
                //when allowSceneActivation is false, unity holds the progress at exactly 90% until ready
                if (op.progress >= 0.9f) {
                    readyForActivation++;
                }
            }
            //we divide by 0.9 here because that is actually the maximum value it can ever be
            ProgressBar.value = sum / _scenesToLoad.Length / 0.9f;
            if (readyForActivation >= _scenesToLoad.Length) {
                foreach (var op in _asyncOperations) {
                    op.allowSceneActivation = true;
                }
                Destroy(this.gameObject);
            }
        }
    }
}
