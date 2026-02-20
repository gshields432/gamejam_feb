using UnityEngine;

public class CloudVolumeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var p = other.GetComponentInParent<NewPlayer>();
        if (p != null) p.cloudVolumeCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var p = other.GetComponentInParent<NewPlayer>();
        if (p != null) p.cloudVolumeCount = Mathf.Max(0, p.cloudVolumeCount - 1);
    }
}