using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public float acceleration = 10f;
    public Transform groundCheck;
    public float maxBonusJumps = 1f;
    private float jumps;
    public float dashSpeed = 10f;

    private float currentSpeed;

    // Time it takes to reach zero speed (in seconds)
    public float speedDecreaseTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        jumps = maxBonusJumps;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        if ((isGrounded || jumps > 0) && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            jumps = isGrounded ? maxBonusJumps : jumps - 1;
        }
        if (!isGrounded && rb.velocity.y <= 0)
        {
            // Start applying a negative velocity to fall faster
            rb.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        float vertical = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f);

        // Apply horizontal movement
        float targetHorizontalSpeed = horizontal * speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetHorizontalSpeed, acceleration * Time.deltaTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
