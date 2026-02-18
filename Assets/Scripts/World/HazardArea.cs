using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HazardArea : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageInterval = 1f;
    private float damageTimer;
    private NewPlayer playerInside;

    private void Update()
    {
        if (playerInside == null)
        {
            return;
        }

        if (damageTimer >= damageInterval)
        {
            playerInside.GetHurt(0, damage);
            damageTimer = 0f;
        }

        damageTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NewPlayer player = collision.GetComponent<NewPlayer>();
        if (player != null)
        {
            playerInside = player;
            damageTimer = 0f;
            playerInside.GetHurt(0, damage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        NewPlayer player = collision.GetComponent<NewPlayer>();
        if (player != null && player == playerInside)
        {
            playerInside = null;
        }
    }
}
