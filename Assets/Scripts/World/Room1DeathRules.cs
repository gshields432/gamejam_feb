using UnityEngine;
using TMPro;

[RequireComponent(typeof(NewPlayer))]
public class Room1DeathRules : MonoBehaviour
{
    [Header("Respawn")]
    public Vector2 respawnPos = new Vector2(-95f, -19f);
    public float respawnDelay = 0.15f;

    [Header("Rules")]
    public bool cloudKillsWhenEyesClosed = true;
    public bool groundKillsWhenEyesOpen = true;
    public float minTimeBetweenDeaths = 0.1f;
    public float minFallSpeedToDie = 20f;

    NewPlayer player;
    float lastDeathTime;
    [SerializeField] private TMP_Text blinkInstructions; // show when dying to Ground
    [SerializeField] private TMP_Text cloudWarning;      // show when dying to Cloud

    void Awake()
    {
        player = GetComponent<NewPlayer>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        HandleContact(col.collider);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        HandleContact(col);
    }

    void HandleContact(Collider2D col)
    {
        if (player == null || col == null) return;
        if (player.dead) return;
        if (Time.time - lastDeathTime < minTimeBetweenDeaths) return;

        bool eyesClosed = GameManager.Instance != null &&
                          GameManager.Instance.GameState == GameManager.GameStates.ClosedEyes;

        Debug.Log($"Hit: {col.tag} | EyesClosed: {eyesClosed} | VelY: {player.velocity.y}");

        // CLOUD
        if (col.CompareTag("Cloud"))
        {
            // Open eyes is safe
            // Closed eyes means death
            if (eyesClosed && cloudKillsWhenEyesClosed && IsFallingFastEnough())
            {
                cloudWarning.gameObject.SetActive(true);
                blinkInstructions.gameObject.SetActive(false);
                DieAndRespawn();
            }
            return;
        }

        // GROUND
        if (col.CompareTag("Ground"))
        {
            // Open eyes means death
            // Closed eyes is safe
            if (!eyesClosed && groundKillsWhenEyesOpen && IsFallingFastEnough())
            {
                cloudWarning.gameObject.SetActive(false);
                blinkInstructions.gameObject.SetActive(true);
                DieAndRespawn();
            }
            return;
        }
    }

    void DieAndRespawn()
    {
        lastDeathTime = Time.time;

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        if (player == null) return;
        player.HardRespawnAt(respawnPos);
    }

    bool IsFallingFastEnough()
    {
        if (player == null) return false;
        return player.maxDownSpeedThisAir <= -minFallSpeedToDie;
    }
}
