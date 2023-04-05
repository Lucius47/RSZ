using UnityEngine;
[ExecuteInEditMode]
public class TestLight : MonoBehaviour
{
    [SerializeField] private bool forceVertex;
    private Light _light;

    private void Start()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        if (forceVertex)
        {
            _light.renderMode = LightRenderMode.ForceVertex;
        }
        else
        {
            _light.renderMode = LightRenderMode.ForcePixel;
        }
    }
}
