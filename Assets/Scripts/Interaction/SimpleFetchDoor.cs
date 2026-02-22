using UnityEngine;

public class SimpleFetchDoor : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private string requiredItem = "key";
    [SerializeField] private bool consumeItemOnOpen = true;

    [Header("Door")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openBoolName = "OpenDoor";

    [Header("Optional Feedback")]
    [SerializeField] private Door speechBubble;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip lockedSound;

    private bool _playerInRange;
    private bool _opened;

    private void Awake()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (!_playerInRange || _opened)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpen();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = true;
            SetPrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = false;
            SetPrompt(false);
        }
    }

    private void TryOpen()
    {
        if (!HasRequiredItem())
        {
            PlayOneShot(lockedSound);
            Debug.Log("You need a key");
            return;
        }
        else
        {
            Debug.Log("Unlocked");
        }

        _opened = true;
        SetPrompt(false);

        if (!string.IsNullOrWhiteSpace(openBoolName) && doorAnimator != null)
        {
            doorAnimator.SetBool(openBoolName, true);
        }

        if (consumeItemOnOpen && !string.IsNullOrWhiteSpace(requiredItem))
        {
            GameManager.Instance.RemoveInventoryItem(requiredItem);
        }

        PlayOneShot(openSound);
    }

    private bool HasRequiredItem()
    {
        if (string.IsNullOrWhiteSpace(requiredItem))
        {
            return true;
        }

        return GameManager.Instance.inventory.ContainsKey(requiredItem);
    }

    private void SetPrompt(bool active)
    {
        if (speechBubble != null)
        {
            if (active)
            {
                speechBubble.ShowSpeechBubble();
            }
            else
            {
                speechBubble.HideSpeechBubble();
            }
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(clip);
        }
    }
}
