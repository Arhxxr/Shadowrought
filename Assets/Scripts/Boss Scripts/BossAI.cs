using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    /*
     * 
     *  ~~ Boss Notes and ToDo ~~
     *  - Boss should follow player from an offset
     *  - Boss should "attack" by spawning a collider that will do scripted damage to player.
     *  - Boss can teleport around room.
     * 
     * 
     * 
     * 
     * 
     * 
     */

    [SerializeField] Rigidbody2D bossRB;
    [SerializeField] SpriteRenderer bossSprite;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] GameObject Boss;
    [SerializeField] GameObject[] warpPoints;

    //Prefabs for boss attacks
    [SerializeField] GameObject bossShortAttack;
    [SerializeField] SpriteRenderer shortAttackSprite;

    [SerializeField] GameObject bossLongAttack;
    [SerializeField] SpriteRenderer longAttackSprite;

    public int bossHP = 10;

    private GameObject bossTeleport;
    private GameObject currentShortAttack;
    private GameObject currentLongAttack;

    private float playerX;
    private float playerY;
    private float playerDistance;// Distance in X value from player to boss

    void Start()
    {
        StartCoroutine(bossHandler());
    }

    void Update()
    {
        //Get distance from player to boss.  Idea is it should be negative if the player is right and vice versa.  We then want to update the sprite
        playerX = playerRB.position.x;
        playerY = playerRB.position.y;

        playerDistance = playerX - bossRB.position.x; //This will get distance on the X axis

        if (playerDistance < 0 )
        {
            bossSprite.flipX = true;
        } else
        {
            bossSprite.flipX = false;
        }

        if (bossHP < 1)
        {
            Debug.Log("Boss Has Died");
            Destroy(Boss);
            Application.Quit();
        }
    }
    IEnumerator bossHandler()
    {
        while (true)
        {
            // Grab random warp gameobject
            bossTeleport = GetClosestWarpPoint();

            //Teleport boss to that game object
            bossRB.position = bossTeleport.transform.position;

            //Wait an amount of seconds
            yield return new WaitForSeconds(0.5f);

            // Do attack then start the loop again
            doLongAttack();

            yield return new WaitForSeconds(1f);

            Destroy(currentShortAttack);
            Destroy(currentLongAttack);
        }
    }

    void doShortAttack()
    {
        Debug.Log("SHORT ATTACKING!!! HYYAAAAAA");
        if (playerDistance < 0)
        {
            shortAttackSprite.flipX = true;
            currentShortAttack = Instantiate(bossShortAttack, new Vector2(bossRB.position.x - 4, bossRB.position.y -10), Quaternion.identity);
        }
        else
        {
            shortAttackSprite.flipX = false;
            currentShortAttack = Instantiate(bossShortAttack, new Vector2(bossRB.position.x + 4, bossRB.position.y -10), Quaternion.identity);
        }
        //We need to instantiate a prefab for the attack toward the position of the player.
        
        
    }
    void doLongAttack()
    {
        if (playerDistance < 0)
        {
            longAttackSprite.flipX = true;
            currentShortAttack = Instantiate(bossLongAttack, new Vector2(bossRB.position.x - 4, bossRB.position.y - 10), Quaternion.identity);
        }
        else
        {
            longAttackSprite.flipX = false;
            currentShortAttack = Instantiate(bossLongAttack, new Vector2(bossRB.position.x + 4, bossRB.position.y -10), Quaternion.identity);
        }
    }

    GameObject returnRandomWarp()
    {
        if (warpPoints != null && warpPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, warpPoints.Length);
            return warpPoints[randomIndex];
        }
        return null;
    }
    GameObject GetClosestWarpPoint()
    {
        if (warpPoints == null || warpPoints.Length == 0)
        {
            return null;
        }

        GameObject closestWarpPoint = null;
        float minDistance = Mathf.Infinity;
        Vector2 playerPosition = playerRB.position;

        foreach (GameObject warpPoint in warpPoints)
        {
            float distance = Vector2.Distance(playerPosition, warpPoint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWarpPoint = warpPoint;
            }
        }

        return closestWarpPoint;
    }
}
