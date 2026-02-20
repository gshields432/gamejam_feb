using UnityEngine;

public class DoorLinker : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;

    private bool _playerInRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != NewPlayer.Instance.gameObject)
        {
            return;
        }

        if (Input.GetButtonDown("Submit"))
        {
            TryEnter(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = false;
        }
    }

    private void TryEnter(Transform triggerPlayer)
    {
        if (!_playerInRange)
        {
            return;
        }

        if (_target == null)
        {
            Debug.LogWarning($"{name}: DoorLinker target is not assigned.", this);
            return;
        }

        Transform playerToMove = _player != null ? _player : triggerPlayer;
        playerToMove.position = _target.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(_target.position, Vector2.one);
    }
}
