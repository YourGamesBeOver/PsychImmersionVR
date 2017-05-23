using UnityEngine;

namespace PsychImmersion.DebugScripts
{
    public class DebugInstantiate : MonoBehaviour
    {

        public GameObject Prefab;
        public KeyCode Key;
	
        // Update is called once per frame
        private void Update () {
            if (!Input.GetKeyDown(Key)) return;
            Instantiate(Prefab, this.gameObject.transform, false);
            this.enabled = false;
        }

#if UNITY_EDITOR
        [ContextMenu("Instantiate Now")]
        private void InstantiateNowEditor()
        {
            Instantiate(Prefab, this.gameObject.transform, false);
        }
#endif
    }
}
