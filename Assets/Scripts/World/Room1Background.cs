using UnityEngine;

public class Room1Background : MonoBehaviour
{
    [Tooltip("Leave empty to use Main Camera.")]
    public Transform cam;
    public float yMultiplier = 0.2f;

    public float xOffset = 0f;
    public float yOffset = 0f;

    float startCamY;
    float startY;

    void Start()
    {
        if (!cam) cam = Camera.main.transform;
        startCamY = cam.position.y;
        startY = transform.position.y;
    }

    void LateUpdate()
    {
        float camYDelta = cam.position.y - startCamY;

        transform.position = new Vector3(
            cam.position.x + xOffset,
            startY + camYDelta * yMultiplier + yOffset,
            transform.position.z
        );
    }
}
