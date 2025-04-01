using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bossAttackDamage : MonoBehaviour
{
    [SerializeField] GameObject player;

    private BoxCollider2D attackHitBox;

    // Start is called before the first frame update
    void Start()
    {
        attackHitBox = GetComponent<BoxCollider2D>();
        PlayerData playerData = player.GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
            playerData.playerHealth--;
        }
    }
}
