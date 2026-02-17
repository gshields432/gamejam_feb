using System.Collections.Generic;
using UnityEngine;

public class VisionEmitter : MonoBehaviour
{
    private static readonly HashSet<VisionEmitter> ActiveEmitterSet = new HashSet<VisionEmitter>();
    private static readonly List<VisionEmitter> ActiveEmitterList = new List<VisionEmitter>();

    [Min(0f)] public float radius = 3f;
    [Min(0f)] public float intensity = 1f;
    public Vector3 offset = Vector3.zero;

    public static IReadOnlyList<VisionEmitter> ActiveEmitters => ActiveEmitterList;

    private void OnEnable()
    {
        if (ActiveEmitterSet.Add(this))
        {
            ActiveEmitterList.Add(this);
        }
    }

    private void OnDisable()
    {
        if (ActiveEmitterSet.Remove(this))
        {
            ActiveEmitterList.Remove(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.7f);
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
