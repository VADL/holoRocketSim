using UnityEngine;

public class SizeControl : MonoBehaviour
{
    private const float maxSize = 10f;
    private const float minSize = 0.01f;

    private void UpdateSize(float newSize)
    {
        float size = Mathf.Clamp(newSize, minSize, maxSize);
        // update scale here for everything
        transform.localScale = Vector3.one * size;
        // need to actually figure out how to update mass / thrust and such for rocket
    }

    // Called by SpeechManager when the user says the "Drop sphere" command
    void OnIncreaseSize()
    {
        float size = transform.localScale.x;
        UpdateSize(size * 2.0f);
    }

    // Called by SpeechManager when the user says the "Drop sphere" command
    void OnDecreaseSize()
    {
        float size = transform.localScale.x;
        UpdateSize(size * 0.5f);
    }
}