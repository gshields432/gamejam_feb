using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Bookshelf : PhysicsObject
{
    [Header ("Reference")]
    public EnemyBase enemyBase;
    private BoxCollider2D boxcollider;
    [SerializeField] private GameObject graphic;

    [Header ("Properties")]
    private Animator animator;

    [SerializeField] private LayerMask layerMask; //What can the Walker actually touch?
    public enum EnemyType{ Bug, Zombie }; //Bugs will simply patrol. Zombie's will immediately start chasing you forever until you defeat them.
    [SerializeField] private EnemyType enemyType;
   
    public float attentionRange;
    public float changeDirectionEase = 1; //How slowly should we change directions? A higher number is slower!
    [System.NonSerialized] public float direction = 1;
    private Vector2 distanceFromPlayer; //How far is this enemy from the player?
    [System.NonSerialized] public float directionSmooth = 1; //The float value that lerps to the direction integer.
    [SerializeField] private bool followPlayer;
    [SerializeField] private bool flipWhenTurning = false; //Should the graphic flip along localScale.x?
    private RaycastHit2D ground;
    public float hurtLaunchPower = 10; //How much force should be applied to the player when getting hurt?
    [SerializeField] private bool jumping;
    public float jumpPower = 7;
    [System.NonSerialized] public bool jump = false;
    [System.NonSerialized] public float launch = 1; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    public float maxSpeed = 7;
    [SerializeField] private float maxSpeedDeviation; //How much should we randomly deviate from maxSpeed? Ensures enemies don't move at exact same speed, thus syncing up.
    [SerializeField] private bool neverStopFollowing = false; //Once the player is seen by an enemy, it will forever follow the player.
    private Vector3 origScale;
    [SerializeField] private Vector2 rayCastSize = new Vector2(1.5f, 1); //The raycast size: (Width, height)
    private Vector2 rayCastSizeOrig;
    [SerializeField] private Vector2 rayCastOffset;
    private RaycastHit2D rightWall;
    private RaycastHit2D leftWall;
    private RaycastHit2D rightLedge;
    private RaycastHit2D leftLedge;

    private float sitStillMultiplier = 1; //If 1, the enemy will move normally. But, if it is set to 0, the enemy will stop completely. 
    [SerializeField] private bool sitStillWhenNotFollowing = false; //Controls the sitStillMultiplier

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip stepSound;
    
    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        animator = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
        //Physics2D.IgnoreCollision(NewPlayer.Instance.GetCollider(), boxcollider);
      
        origScale = transform.localScale;
        rayCastSizeOrig = rayCastSize;
        maxSpeed -= Random.Range(0, maxSpeedDeviation);
        launch = 0;
        if (enemyType == EnemyType.Zombie)
        {
            direction = 0;
            directionSmooth = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attentionRange);
    }

    private void Update()
    {
        ComputeVelocity();
        handleEyes();
    }
    void handleEyes() {
        //BUG: when detransforming, the animation plays twice
        //Debug.Log("distanceFromPlayer: " + distanceFromPlayer);
        
        if (Mathf.Abs(distanceFromPlayer.x) < attentionRange)
        {
            //animator.SetTrigger("Fall");
        }
        else
        {
            
        }
        if (GameManager.Instance.GameState.Equals(GameManager.GameStates.OpenEyes)) {
            //prevState = GameManager.GameStates.OpenEyes;
            animator.SetTrigger("Transform");
            
            //Debug.Log("animator.IsInTransition(0): "+animator.IsInTransition(0));
            //attentionRange = 10000000000;
            
        }
        else
        {
            //animator.SetBool("isWalking", false);
            //if(prevState.Equals(GameManager.GameStates.OpenEyes))
            //{

            //    prevState = GameManager.GameStates.ClosedEyes;
            //}
            animator.SetTrigger("Get Up");
            animator.SetTrigger("Detransform");
        }
    
    }
    protected void ComputeVelocity()
    {
        //bookshelf doesn't move
        }

    public void Jump()
    {

    }

    public void PlayStepSound()
    {
        enemyBase.audioSource.pitch = (Random.Range(0.6f, 1f));
        enemyBase.audioSource.PlayOneShot(stepSound);
    }

    public void PlayJumpSound()
    {
        //enemyBase.audioSource.pitch = (Random.Range(0.6f, 1f));
        //enemyBase.audioSource.PlayOneShot(jumpSound);
    }

}