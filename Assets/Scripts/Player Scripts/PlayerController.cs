using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    /* 
     * TODO:
     *  Player Powers
     *      Double Jump
     *      Dash
     *      Passthrough ability (??)
     *  Save System
     */

    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Collider2D groundDetect; //Collider of the Sprite itself

    //UI STUFF
    [SerializeField] GameObject pauseMenuPanel;
    private bool isPaused;

    public float maxSpeed = 5f; //used for left and right movement
    public float jumpHeight = 6f; //used for Jump speed/height
    public float jumpCount = 0; // I mean.. self explanitory.

    private float coyoteTime = 0.2f; //used for coyote time (yet to be balanced/tested)
    private float coyoteTimeCounter; 

    private float jumpBufferTime = 0.2f; //used for jump buffer (yet to be balanced/tested)
    private float jumpBufferCounter;

    public bool canDoubleJump = false; //used to determine whether player has unlocked doubleJump
    private bool doubleJumpAvail = true; //used to determine whether player can make a second jump while in game

    public bool dashUnlocked = false;
    public bool canDash = true; //used to determine whether player has unlocked dash
    private bool isDashing; //used to determine whether player is dashing while in game
    public float dashSpeed = 2000f; 
    private float dashingTime = 0.5f;
/*    private float dashCooldown = 1f;*/
/*    private int dashCounter = 2;*/
    private Vector2 dashDir;

    //Raycast variables
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    // Place to store movement inputs.
    private float moveInput;
    private float previousMoveInput;

    // Power unlock information script reference.
    private PlayerData playerData;

    public GameObject target; //Power unlock target

    [Header("Audio")]
    [SerializeField] AudioClip FootstepSound, JumpSound, DashSound, AttackSound;
    private AudioSource audioSource;
    private bool FootstepSoundPlayed;
    private bool JumpSoundPlayed;
    private bool DashSoundPlayed;

    [Header("Attacking")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;
    [SerializeField] Transform AttackTransform;
    [SerializeField] Vector2 AttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float AttackDamage;
    public bool canAttack = true;

    //Animator variables
    Animator anim;
    private bool isRunning;
    private bool isJumping;
    private bool isFalling;
    private bool isTurning;
    private bool isFacingRight = true; //flag to check flip

    public bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    }

    //Visualize Raycast for debug and testing.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
        Gizmos.color = Color.red;
    }
    public static PlayerMovement Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        playerData = GetComponent<PlayerData>();
/*        Debug.Log("Double Jump Unlocked:" + playerData.doubleJumpUnlocked);*/
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
/*        if (GameManager.Instance.gameIsPaused)
        {
            return;
        }*/

        if (isDashing)
        {
            rb.gravityScale = 0f;
            return;
        }
        moveInput = Input.GetAxis("Horizontal"); //Get input from the user on a scale of -1 to 1
        rb.velocity = new Vector2(moveInput * maxSpeed, rb.velocity.y); //Change left and right velocities

        //Verifies horizontal ground movement for running animation parameter
        anim.SetBool("isRunning", rb.velocity.x !=0 && isGrounded());

        //logic for coyote time
        if (isGrounded())
        {   
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //logic for input jump buffer
        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        /* -- Double Jump Logic --
         * 
         *  Check if double jump is unlocked.  If it is, allow one more jump, then only allow jump once grounded.
         * 
         */
        // I know i dont need to specify == true but idk not working so here we gooooooo
        if (!canDoubleJump)
        {
            if (Input.GetButtonUp("Jump") && rb.velocity.y > jumpHeight / 2f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 1.5f); //Variable Jump

                coyoteTimeCounter = 0;
            }
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f) //conditions here are like this for the coyote time and jump buffer logics
            {
                audioSource.PlayOneShot(JumpSound);
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //Initiate the jump by changing velocities   

                jumpBufferCounter = 0f;
            }
        }
        // Double Jump Logic here
        else 
        {
            if (isGrounded() && !Input.GetButton("Jump")) //when the player is not pressing jump
            {
                doubleJumpAvail = false;
                jumpBufferCounter = 0f;
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > jumpHeight / 2f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 1.5f); //Variable Jump

                coyoteTimeCounter = 0;
            }
            if (jumpBufferCounter > 0f) 
            {
                if (coyoteTimeCounter > 0f || doubleJumpAvail)
                {
                    audioSource.PlayOneShot(JumpSound);
                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //Initiate the jump by changing velocities
                    doubleJumpAvail = !doubleJumpAvail; //player has already jumped once, will either fall or can jump one last time before falling
                    jumpBufferCounter = 0f;
                }
            }
        }

        attack = Input.GetMouseButtonDown(0);
        Attack(); //Uses Attack function written outside of Update

        // Logic for Dash

        if(dashUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                isDashing = true;
                canDash = false;
                dashDir = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);

                //Verifies that player is dashing for dashing animation and sound parameter
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetTrigger("playerDashing");
                }
                audioSource.PlayOneShot(DashSound);

                if (dashDir == Vector2.zero)
                {
                    dashDir = new Vector2(transform.localScale.x, 0f);
                }

                StartCoroutine(StopDashing());
            }

            if (isDashing && isFacingRight)
            {

                float oldGravityScale = rb.gravityScale;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
                rb.gravityScale = oldGravityScale;
                return;
            }

            if (isDashing && !isFacingRight)
            {
                float oldGravityScale = rb.gravityScale;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
                rb.gravityScale = oldGravityScale;
                return;
            }

            if (isGrounded())
            {
                canDash = true;
            }
        }

        // End of Dash Logic



        //Verifies vertical movement for jumping animation parameters
        anim.SetBool("isJumping", rb.velocity.y > 0 && !isGrounded());
        anim.SetBool("isFalling", rb.velocity.y < 0 && !isGrounded());

        // Check if player is turning
        if ((moveInput > 0 && previousMoveInput < 0) || (moveInput < 0 && previousMoveInput > 0))
        {
            isTurning = true;
        }
        else
        {
            isTurning = false;
        }

        anim.SetBool("isTurning", isTurning);

        previousMoveInput = moveInput;

        // Variable gravity for after apex of jump
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 22f;
        } else
        {
            rb.gravityScale = 7f;
        }

        // Menu Stuff (Save/Quit/Options)
        // Need to bring up an additional UI if the player hits escape that has at least a quit and continue option
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                pauseMenuPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                pauseMenuPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    //Logic for Attack
    void Attack()
    {
        if (canAttack)
        { 
            timeSinceAttack += Time.deltaTime;
            if (attack && timeSinceAttack >= timeBetweenAttack)
            {
                timeSinceAttack = 0;
                anim.SetTrigger("Attacking");
                audioSource.PlayOneShot(AttackSound);
            }
        }
    }

    //Confirms Hit
    private void Hit (Transform AttackTransform, Vector2 AttackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(AttackTransform.position, AttackArea, 0, attackableLayer);
            
    }
    //End of Attack Logic


    //CoRoutine for stopping Dash
    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
    }

    private void FixedUpdate()
    {
        /*//Clamping player fall speed on the y axis
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, maxFallSpeed * 5));*/

        if (moveInput > 0 || moveInput < 0)
        {
            TurnCheck();
        }
    }

    private void TurnCheck() //Executes Turn over Y-axis rotation
    {
        if (moveInput > 0 && !isFacingRight)
        {
            Turn();
        }
        if (moveInput < 0 && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn() //Turn Logic, uses Y-axis rotation instead of X-axis flip.
                        //This ensures that the transform data also flips
    {
        if (isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }

        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
    }

    public void SaveGame()
    {
        // Save player health and level
        PlayerPrefs.GetInt("playerHealth", playerData.playerHealth); // Save Player Health
        PlayerPrefs.GetString("currentLevel", SceneManager.GetActiveScene().name); // Save Players Current Level

        // Save position on screen.  We have to do this because we are not using JSON to save.  Will have to implement JSON later if game is used.
        PlayerPrefs.GetFloat("PlayerXCoord", transform.position.x);
        PlayerPrefs.GetFloat("PlayerYCoord", transform.position.y);

        // Save unlock of extra abilities (bool?)

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {

    }
}
