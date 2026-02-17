using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.3f; //How long it takes to fade in/out

    [Header("Pop")]
    [SerializeField] private float popDuration = 0.35f; // How long scale pop takes

    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.1f; // How far speech bubble moves up and down on idle
    [SerializeField] private float hoverDuration = 1f; // how long movement takes

    private SpriteRenderer[] spriteRenderers;
    private Tween currentTween;
    private Tween hoverTween;

    private Vector3 originalScale;
    private Vector3 startPosition;

    private bool isVisible = false;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
        startPosition = transform.localPosition;

        SetAlpha(0f);
        transform.localScale = Vector3.zero;

        SetupHover();
        hoverTween.Pause();
    }

/*     void Update()
    {
        // TEST press space to toggle speech bubble 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isVisible)
                HideSpeechBubble();
            else
                ShowSpeechBubble();

            isVisible = !isVisible;
        }
    } */

    void SetupHover() //Creates the looping hover animation
    {
        hoverTween = transform
            .DOLocalMoveY(startPosition.y + hoverHeight, hoverDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ShowSpeechBubble() //call this to show the speech bubble
    {
        KillTween();

        transform.localScale = Vector3.zero;
        SetAlpha(0f);

        Sequence seq = DOTween.Sequence();

        foreach (var sr in spriteRenderers)
        {
            seq.Join(sr.DOFade(1f, fadeDuration));
        }

        seq.Join(transform
            .DOScale(originalScale, popDuration)
            .SetEase(Ease.OutBack));

        seq.OnComplete(() => hoverTween.Play());

        currentTween = seq;
    }

    public void HideSpeechBubble() //call this to hide the speech bubble
    {
        KillTween();
        hoverTween.Pause();

        Sequence seq = DOTween.Sequence();

        foreach (var sr in spriteRenderers)
        {
            seq.Join(sr.DOFade(0f, fadeDuration));
        }

        seq.Join(transform
            .DOScale(Vector3.zero, fadeDuration)
            .SetEase(Ease.InBack));

        currentTween = seq;
    }

    private void SetAlpha(float value) //sets alpha for sprite renderers
    {
        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = value;
            sr.color = c;
        }
    }

    private void KillTween() //kills any currently running tweens
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();
    }
}