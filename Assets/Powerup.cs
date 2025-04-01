using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{


    public bool doubleJump = false;
    public bool dash = false;
    public bool attack = false;

    public GameObject pickupEffect;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Ability Unlocked!");
            Instantiate(pickupEffect, transform.position, transform.rotation);

            if (doubleJump)
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                playerMovement.canDoubleJump = true;
            }

            if (dash)
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                playerMovement.dashUnlocked = true;
            }

            if (attack)
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
                playerMovement.canAttack = true;
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            Destroy(gameObject);
        }
    }

}
