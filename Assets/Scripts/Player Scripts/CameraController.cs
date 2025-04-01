using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /* Camera MUST:
     *  Have smoothing to make movement not as jittery.
     *  Allow for effects such as screenshake or tilt
     *  
     * Technical:
     *  Get player transform (Position)
     *  Set camera transform (Position) to player transform using a lerp or some other smoothing method with a tiny, fractional delay.
     *  
     *  Lerp should allow camera to lag slightly behind player and then catch up after the player ends.  In order for this to look right, camera should start
     *  in front of player.
     *  
     */

    [SerializeField] Transform playerTransform;
    [SerializeField] SpriteRenderer playerSprite;

    private float smoothing = 5f;
    private float cameraOffset;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get player facing direction

        //Set camera X to be "in front" of player facing direction

        //LERP in between facing directions

        // GET flip status for sprite, and then set the camera look direction based off of that
        if (playerSprite.flipX == true)
        {
            cameraOffset = -5;
        } else
        {
            cameraOffset = 5;
        }

        Vector3 targetPosition = new Vector3(playerTransform.position.x + cameraOffset, playerTransform.position.y, playerTransform.position.z-1); // set obj transform to player transform
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing*Time.deltaTime);
    }
}
