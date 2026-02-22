using UnityEngine;

public class teddy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    private Rigidbody2D rb;
    private bool hasThrown = false; // used to ensure the Throw() only happens once per blink

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(-Physics.gravity, ForceMode2D.Force);
        if (GameManager.Instance.GameState.Equals(GameManager.GameStates.OpenEyes))
        {
            animator.SetTrigger("Transform");
            Throw();
        }
        else
        {
            hasThrown = false;
            animator.SetTrigger("Detransform");
        }
    }

    private void Throw()
    {
        Vector2 throwforce = new Vector2 (5, 7);
        rb.AddForce(throwforce, ForceMode2D.Impulse);
        hasThrown = true;
        
    }
}

