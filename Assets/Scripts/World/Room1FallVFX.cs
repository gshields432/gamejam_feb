using UnityEngine;

public class Room1FallVFX : MonoBehaviour
{
    public NewPlayer player;
    public Material backgroundMaterial;

    [Header("Stretch")]
    public float maxStretch = 0.35f;
    public float smooth = 1f;

    float currentStretch;

    void Update()
    {
        if (!player || !backgroundMaterial) return;
        float target = Mathf.Clamp01(player.fallIntensity) * maxStretch;

        currentStretch = Mathf.Lerp(
            currentStretch,
            target,
            1f - Mathf.Exp(-smooth * Time.deltaTime)
        );

        backgroundMaterial.SetFloat("_Stretch", currentStretch);
    }
}
