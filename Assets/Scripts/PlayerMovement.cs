using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    SpriteRenderer mySpriteRenderer;
    bool isAlive = true;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float gravityOnLadder = 0f;
    [SerializeField] float gravityOffLadder = 4f;
    [SerializeField] float dieJump = 10f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletTransform;
    Animator myAnimator;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if(!isAlive) 
        {
            Debug.Log("Died");
            return;
        }

        Run();
        FlipSprite();
        CheckClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive) {return;}
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump (InputValue value)
    {
        if(!isAlive) {return;}        
        if(value.isPressed && (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))))
        {
            myRigidBody.velocity += new Vector2 (0f, jumpSpeed);
        }
    }
    void OnFire (InputValue value)
    {
        if(!isAlive) {return;}    

        Instantiate(bullet, bulletTransform.position, transform.rotation);

    }
    void Run()
    {
        
        Vector2 playerVelocity = new Vector2 (runSpeed * moveInput.x, myRigidBody.velocity.y) ;
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

    }
    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x), 1f);

        }
    }
    void CheckClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            ClimbLadder();
            myRigidBody.gravityScale = gravityOnLadder;
            myAnimator.SetBool("isOnLadder", true);

        }
        else
        {
            myRigidBody.gravityScale = gravityOffLadder;       
            myAnimator.StopPlayback();
            myAnimator.SetBool("isOnLadder", false);
            myAnimator.SetBool("isClimbing", false);

        }
    }
    void ClimbLadder()
    {
        Vector2 climbVelocity = new Vector2 (runSpeed * moveInput.x, climbSpeed * moveInput.y) ;
        myRigidBody.velocity = climbVelocity;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die() 
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = new Vector2 (0f, dieJump);
            mySpriteRenderer.color = Color.gray;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
