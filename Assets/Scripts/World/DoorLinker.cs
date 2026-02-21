using UnityEngine;

public class DoorLinker : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;

    private bool _playerInRange;
    private Vector2 _landingPos;

    private void Awake()
    {
        var landingPos = _target.position;
        var adjusted = new Vector2(landingPos.x, landingPos.y - 1.8f);
        _landingPos = adjusted;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = true;
            _animator.SetBool("OpenDoor", true);
        }
    }

    private void Update()
    {
        if (!_playerInRange)
        {
            return;
        }

        if (Input.GetButtonDown("Submit"))
        {
            Enter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            _playerInRange = false;
            _animator.SetBool("OpenDoor", false);
        }
    }

    private void Enter()
    {
        if (_target == null)
        {
            Debug.LogWarning($"{name}: DoorLinker target is not assigned.", this);
            return;
        }

        _player.position = _landingPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(_landingPos, Vector2.one);
    }
}
