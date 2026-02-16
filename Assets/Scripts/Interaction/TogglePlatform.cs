using UnityEngine;

public class TogglePlatform : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polyCollider;
    [SerializeField] GameManager.GameStates PlatformType;

    // Update is called once per frame
    void Update()
    {
        //should also update visuals here
        
        // if the game state matches the gamestate of the platform, change the platform to be active
        if(GameManager.Instance.GameState == PlatformType)
        {
            //Active mode: platform is visible and has collision
            polyCollider.enabled = true;
            spriteRenderer.SetAlpha(1.0f);
            
        }
        else
        {  
            //Inactive mode: platform is transparent and has NO collision
            polyCollider.enabled = false;
            spriteRenderer.SetAlpha(0.5f);
        }
    }



}


