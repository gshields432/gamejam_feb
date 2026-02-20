using System.Collections;
using UnityEngine;

public class CloudPlatformPoof : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D solidCollider;

    [Header("Timing")]
    [SerializeField] private float poofDelay = 1.0f;        


    void OnCollisionEnter2D(Collision2D col)
    {
        // Detect player
        var player = col.collider.GetComponentInParent<NewPlayer>();
        if (player == null) return;

        bool eyesClosed = GameManager.Instance != null &&
                          GameManager.Instance.GameState == GameManager.GameStates.Bad;
        if (!eyesClosed) // Don't poof cloud if player has their eyes closed.
        {
            StartCoroutine(PoofRoutine());
        }
    }

    IEnumerator PoofRoutine()
    {
        if (poofDelay > 0f)
            yield return new WaitForSeconds(poofDelay);

        if (animator) animator.SetTrigger("CloudPoof");

        // Disable standing collider
        if (solidCollider) solidCollider.enabled = false;

        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(2f);
        if (solidCollider) solidCollider.enabled = true;
        if (animator) animator.SetTrigger("CloudUnpoof");
    }
}