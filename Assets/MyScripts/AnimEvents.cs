using UnityEngine;

/// <summary>
/// Functions called by animation events
/// </summary>
public class AnimEvents : MonoBehaviour
{
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
