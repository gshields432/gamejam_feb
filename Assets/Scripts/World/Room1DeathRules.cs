using UnityEngine;
using TMPro;
using DG.Tweening;

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

    [Header("Cloud Warning Animation")]

    public float visibleDuration = 4f;
    public float fadeDuration = 0.5f;
    public float floatDistance = 10f;

    Tween currentTween;

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
                PlayFloatingText(cloudWarning);
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
                PlayFloatingText(blinkInstructions);
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

    void PlayFloatingText(TMP_Text text)
{
    if (text == null) return;

    currentTween?.Kill();

    text.gameObject.SetActive(true);

    RectTransform rect = text.rectTransform;

    // This is the position you set in the editor
    Vector2 readablePos = rect.anchoredPosition;

    // Start slightly below it
    Vector2 startPos = readablePos - new Vector2(0, floatDistance * 0.5f);

    rect.anchoredPosition = startPos;

    // Start invisible
    Color c = text.color;
    c.a = 0f;
    text.color = c;

    float slowFloatDistance = floatDistance * 0.5f;

    Sequence seq = DOTween.Sequence();

    // -------- PHASE 1: Fade UP into readable position --------
    seq.Append(text.DOFade(1f, fadeDuration));
    seq.Join(rect.DOAnchorPos(readablePos, fadeDuration)
        .SetEase(Ease.OutQuad));

    // -------- PHASE 2: Slow float above readable position --------
    seq.Append(rect.DOAnchorPosY(readablePos.y + slowFloatDistance, visibleDuration)
        .SetEase(Ease.Linear));

    // -------- PHASE 3: Faster float + fade out --------
    seq.Append(text.DOFade(0f, fadeDuration));
    seq.Join(rect.DOAnchorPosY(readablePos.y + floatDistance, fadeDuration)
        .SetEase(Ease.InQuad));

    seq.OnComplete(() =>
    {
        // Reset back below so it's ready next time
        rect.anchoredPosition = readablePos;
        text.gameObject.SetActive(false);
    });

    currentTween = seq;
}
}
