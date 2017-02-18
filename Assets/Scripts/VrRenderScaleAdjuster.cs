using UnityEngine;
using UnityEngine.VR;

[ExecuteInEditMode]
public class VrRenderScaleAdjuster : MonoBehaviour
{

    [SerializeField]
    [Range(0.5f, 2f)]
    private float _renderScale = 1f;

    public float RenderScale
    {
        get { return _renderScale; }
        set
        {
            _renderScale = value;
            VRSettings.renderScale = value;
        }
    }

    private void OnValidate()
    {
        VRSettings.renderScale = _renderScale;
    }
}
