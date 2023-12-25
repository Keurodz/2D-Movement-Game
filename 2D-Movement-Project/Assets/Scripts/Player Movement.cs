using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float acceleration = 10f;
    public float maxBonusJumps = 2f;
    public float dashSpeed = 10f;

    private Rigidbody2D rb;
    private float currentSpeed;
    private bool isGrounded;
    private bool isTouchingWall;
    private float jumps;
    private bool canDash = true;

    private enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Dashing
    }

    private PlayerState currentState = PlayerState.Idle;

    public Transform groundCheck;
    public Transform wallCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        jumps = maxBonusJumps;
    }

    void Update()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, 0.1f);

        switch (currentState)
        {
            case PlayerState.Idle:
                HandleIdleState();
                break;
            case PlayerState.Walking:
                HandleWalkingState();
                break;
            case PlayerState.Jumping:
                HandleJumpingState();
                break;
            case PlayerState.Dashing:
                HandleDashingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f);

        if (isGrounded) {
            jumps = maxBonusJumps;
        }

        if ((isGrounded || jumps > 0) && Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerState.Jumping;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            currentState = PlayerState.Dashing;
        }

        // Transition to Walking state if there's horizontal input
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            currentState = PlayerState.Walking;
        }
        // Reset currentSpeed when transitioning from Dashing
        if (currentState != PlayerState.Dashing && isGrounded)
        {
          currentSpeed = 0f;
        }

        // Handle deceleration in the idle state
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0f, acceleration * Time.deltaTime), rb.velocity.y);
        }
        // // If touching the wall and not grounded, fall down
        // if (isTouchingWall && !isGrounded)
        // {
        //     rb.velocity = new Vector2(0f, rb.velocity.y);
        // }
    }

    private void HandleWalkingState()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f);

        // Apply horizontal movement
        float targetHorizontalSpeed = horizontal * speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetHorizontalSpeed, acceleration * Time.deltaTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // Transition to Idle state if there's no horizontal input
        if (Mathf.Abs(horizontal) < 0.1f)
        {
            currentState = PlayerState.Idle;
        }

        // Transition to Jumping state if jump key is pressed
        if ((isGrounded || jumps > 0) && Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerState.Jumping;
        }

        // Transition to Dashing state if dash key is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            currentState = PlayerState.Dashing;
        }

        if (isTouchingWall && !isGrounded)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void HandleJumpingState()
    {
        Jump();
        currentState = PlayerState.Idle;
    }

    private void HandleDashingState()
    {
        Dash();
        currentState = PlayerState.Walking;
    }

    private void Jump()
    {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumps -= 1f;
    }

    private void Dash()
    {

    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    if (horizontal != 0f || vertical != 0f)
    {
        Vector2 dashDirection = new Vector2(horizontal, vertical).normalized;
        rb.velocity = dashDirection * dashSpeed;

        currentSpeed = rb.velocity.x;

        canDash = false; // Disable dashing until cooldown is over
        StartCoroutine(DashCooldown());
    }
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(1f);
        canDash = true;
    }
}
