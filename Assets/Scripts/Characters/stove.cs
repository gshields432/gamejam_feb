using UnityEngine;

public class stove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState.Equals(GameManager.GameStates.OpenEyes))
        {
            animator.SetTrigger("Transform");
        }
        else
        {
            animator.SetTrigger("Detransform");
        }
    }
}
