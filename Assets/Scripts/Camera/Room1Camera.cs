using UnityEngine;
using Cinemachine;

public class PlayerFallLookCamera2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private NewPlayer player; // drag your player here (or leave null to auto-find)

    [Header("Look Down")]
    [SerializeField] private float maxLookDown = 4.0f;     // world units of downward offset at full fall
    [SerializeField] private float startFallSpeed = 2.0f;  // should match your player fallMinSpeed
    [SerializeField] private float fallLerpSpeed = 14f;    // how fast camera pans down while falling
    [SerializeField] private float recenterLerpSpeed = 25f;// how fast it recenters on landing
    [SerializeField] private bool snapOnLand = true;

    private CinemachineFramingTransposer framing;
    private float defaultYOffset;
    private bool wasGrounded;

    void Awake()
    {
        if (!vcam) vcam = GetComponent<CinemachineVirtualCamera>();
        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        defaultYOffset = framing.m_TrackedObjectOffset.y;
    }

    void Start()
    {
        if (!player) player = FindObjectOfType<NewPlayer>();
        wasGrounded = player != null && player.grounded;
    }

    void LateUpdate()
    {
        if (!player || framing == null) return;

        bool groundedNow = player.grounded;

        // Your player already computes fallSpeed / fallIntensity:
        // fallSpeed = max(0, -velocity.y)
        // fallIntensity is 0..1 (squared)
        float fallSpeed = player.fallSpeed;
        float fallIntensity = player.fallIntensity; // already 0..1, squared

        // Only look down if actually falling past a threshold
        bool isFalling = !groundedNow && fallSpeed > startFallSpeed;

        float targetYOffset = defaultYOffset;
        if (isFalling)
        {
            // Move downward (negative Y) proportional to fall intensity
            targetYOffset = defaultYOffset - (maxLookDown * fallIntensity);
        }

        // Landing snap (optional)
        if (!wasGrounded && groundedNow && snapOnLand)
        {
            var o = framing.m_TrackedObjectOffset;
            o.y = defaultYOffset;
            framing.m_TrackedObjectOffset = o;
            wasGrounded = groundedNow;
            return;
        }

        float speed = groundedNow ? recenterLerpSpeed : fallLerpSpeed;

        var offset = framing.m_TrackedObjectOffset;
        // Exponential smoothing (frame-rate independent)
        offset.y = Mathf.Lerp(offset.y, targetYOffset, 1f - Mathf.Exp(-speed * Time.deltaTime));
        framing.m_TrackedObjectOffset = offset;

        wasGrounded = groundedNow;
    }
}
