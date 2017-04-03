using UnityEngine;

namespace PsychImmersion
{
    [RequireComponent(typeof(Camera))]
    public class RenderDepthTexture : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
            Destroy(this);
        }
    }
}
