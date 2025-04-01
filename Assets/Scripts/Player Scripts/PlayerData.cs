using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthCounterText;
    [SerializeField] Transform playerTransform; //For saving player exact position.  Maybe not used in favor of save points?
    
    public int playerHealth = 10;
    public bool doubleJumpUnlocked = true;
    public float jumpsAvailable = 1f;

    //private string currentLevel = "DevelopmentLevel"; //this is to keep track of where the player is in the game when they die.
    private GameObject currentSavePoint; //This may not be used in favor of a save-anywhere system.

    private void Awake()
    {
        Debug.Log("PlayerData Awake: Double Jump Status:" + doubleJumpUnlocked);
    }

    private void Update()
    {
        if (playerHealth < 1)
        {
            Debug.Log("PLAYER HAS DIED... respawning");

            // We cannot destroy the player until we have amended all checks that reference the player. Otherwise it throws many errors.
            // Destroy(gameObject);

            // Maybe respawn the player at a specific location
            gameObject.transform.position = new Vector3(0, 15, 0);
            playerHealth = 10;
        }
        //healthCounterText.text = playerHealth.ToString();
    }
}
