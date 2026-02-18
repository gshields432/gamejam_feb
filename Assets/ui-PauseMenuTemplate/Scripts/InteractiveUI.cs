using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class UIInteractable : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Button targetButton;
    [SerializeField] private AudioClip[] audioClips;

    [Header("Animation Settings")]
    public float hoverY = 10f;
    public float pressY = -5f;
    public float animationDuration = 0.2f;

    private Vector2 originalAnchoredPos;
    private RectTransform rect;
    public bool isHovered = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (targetButton == null)
            targetButton = GetComponent<Button>();

        originalAnchoredPos = rect.anchoredPosition;
    }

    // --- Hover + Press Logic ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!targetButton.interactable) return;
        isHovered = true;
        SoundFXManager.instance.PlayRandomSoundFXClip(audioClips, transform, 1f);
        rect.DOComplete();
        rect.DOAnchorPosY(originalAnchoredPos.y + hoverY, animationDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!targetButton.interactable) return;

        isHovered = false;
        rect.DOComplete();
        rect.DOAnchorPosY(originalAnchoredPos.y, animationDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!targetButton.interactable) return;
        rect.DOComplete();
        rect.DOAnchorPosY(originalAnchoredPos.y + pressY, animationDuration * 0.5f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!targetButton.interactable) return;

        rect.DOComplete();
        if (isHovered)
            rect.DOAnchorPosY(originalAnchoredPos.y + hoverY, animationDuration).SetEase(Ease.OutQuad);
        else
            rect.DOAnchorPosY(originalAnchoredPos.y, animationDuration).SetEase(Ease.OutQuad);
    }
}
