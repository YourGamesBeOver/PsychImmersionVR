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
    }
}
