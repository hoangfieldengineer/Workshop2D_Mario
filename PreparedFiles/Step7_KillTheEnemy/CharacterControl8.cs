using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl8 : MonoBehaviour
{
    public float moveSpeed = 8f;
    private Rigidbody2D m_rigidbody2D;
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;
    private float m_currentSpeed;

    private bool m_jump = false;
    public float jumpSpeed = 12;

    private bool m_isGround = true;
    public Transform m_GroundCheck;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    private float k_GroundedRadius = 0.26f; // Radius of the overlap circle to determine if grounded
    public float m_maxJumpDuration = 0.3f;
    private float m_accumulatedDurationSinceJump = 0f;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_isGround)
        {
            m_jump = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            m_jump = false;
        }

        if (m_jump)
        {
            if(!m_isGround && m_accumulatedDurationSinceJump >= m_maxJumpDuration)
            {                
                m_jump = false;
            }
        }
    }

    void SetSpeedY(float speedY)
    {
        Vector2 vel = m_rigidbody2D.velocity;
        vel.y = speedY;
        m_rigidbody2D.velocity = vel;
    }

    void FixedUpdate()
    { 
        if (!m_isLive)
        {
            return;
        }

        m_isGround = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_isGround = true;
        }
        m_animator.SetBool("Ground", m_isGround);

        if (m_jump)
        {
            SetSpeedY(jumpSpeed);
            m_accumulatedDurationSinceJump += Time.deltaTime;
        }
        else {
            m_accumulatedDurationSinceJump = 0f;
        }

        float horizontalMove = Input.GetAxis("Horizontal"); // use arrow keys on keyboard will cause this variable changed, vary from -1 to 1
        // Debug.Log("horizontalMove: " + horizontalMove);
        m_currentSpeed = horizontalMove * moveSpeed;
        m_rigidbody2D.velocity = new Vector2(m_currentSpeed, m_rigidbody2D.velocity.y);
        m_animator.SetFloat("Speed",  Mathf.Abs(horizontalMove));

         // If the input is moving the player right and the player is facing left...
        if (horizontalMove > 0 && m_spriteRenderer.flipX)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (horizontalMove < 0 && !m_spriteRenderer.flipX)
        {
            // ... flip the player.
            Flip();
        }
    }
    void Flip()
    {
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
    }

    private bool m_isLive = true;
    void Die()
    {
        if (!m_isLive)
        {
            return;
        }

        m_rigidbody2D.velocity = new Vector2(0f, m_rigidbody2D.velocity.y);
        gameObject.layer = LayerMask.NameToLayer("CameraBoundary");
        m_isLive = false;
        m_animator.SetBool("Ground", true);
        m_animator.SetTrigger("Death");
    }
}
