using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl2 : MonoBehaviour
{
    public Transform m_GroundCheck;
    public float m_jumpSpeedMin = 11f;
    public float m_jumpSpeedMax = 12f;
    public float m_maxJumpDuration = 0.3f;
    public float m_moveSpeed = 7f;
    public float m_moveSpeedFastMode = 11f;
    public float m_accelerationSpeed = 9f;
    public float m_decelerationSpeed = 12f;
    public bool m_moveFast = false;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    const float k_GroundedRadius = 0.26f; // Radius of the overlap circle to determine if grounded

    private SpriteRenderer m_spriteRenderer;
    private Rigidbody2D m_rigidBody2D;
    private Animator m_animator;
    private float m_accumulatedDurationSinceJump = 0f;
    private float m_speedHorizontal;
    private bool m_jump = false;
    private bool m_isGround = true;
    private bool m_isLive = true;

    private float Speed
    {
        get { return m_moveFast ? m_moveSpeedFastMode : m_moveSpeed; }
    }

    void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isLive)
        {
            return;
        }

        m_moveFast = Input.GetKey(KeyCode.A);

        if (Input.GetButtonDown("Jump") && m_isGround)
        {
            m_jump = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            m_jump = false;
        }

        if (m_jump)
        {
            if (!m_isGround && m_accumulatedDurationSinceJump >= m_maxJumpDuration)
            {
                m_jump = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (!m_isLive)
        {
            return;
        }

        m_isGround = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_isGround = true;
        }
        m_animator.SetBool("Ground", m_isGround);

        if (m_jump)
        {
            float force = Mathf.Lerp(m_jumpSpeedMin, m_jumpSpeedMax, m_accumulatedDurationSinceJump / m_maxJumpDuration);
            SetSpeedY(force);
            m_accumulatedDurationSinceJump += Time.deltaTime;
        }
        else if (m_isGround)
        {
            m_accumulatedDurationSinceJump = 0f;
        }

        m_animator.SetFloat("vSpeed", m_rigidBody2D.velocity.y);

        float horizontalMove = Input.GetAxis("Horizontal");

        float sign = Mathf.Sign(horizontalMove);

        if (Mathf.Abs(horizontalMove) > 0.001f)
        {
            float targetSpeed = sign * Speed;
            float maxDeltaTime = Time.deltaTime *
                                    (m_accelerationSpeed +
                                    (Mathf.Sign(m_speedHorizontal) != sign ? m_decelerationSpeed : 0));

            m_speedHorizontal = Mathf.MoveTowards(m_speedHorizontal, targetSpeed, maxDeltaTime);

            // If the input is moving the player right and the player is facing left...
            if (sign > 0 && m_spriteRenderer.flipX)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (sign < 0 && !m_spriteRenderer.flipX)
            {
                // ... flip the player.
                Flip();
            }
        }
        else
        {
            m_speedHorizontal = Mathf.MoveTowards(m_speedHorizontal, 0f, Time.deltaTime * m_decelerationSpeed);
        }

        m_animator.SetFloat("Speed", Mathf.Abs(m_speedHorizontal) / m_moveSpeed);
        m_animator.SetBool("Ground", m_isGround);
        curveSpeedX.AddKey(Time.realtimeSinceStartup, m_speedHorizontal);
        m_rigidBody2D.velocity = new Vector2(m_speedHorizontal, m_rigidBody2D.velocity.y);
    }

    public AnimationCurve curveSpeedX = new AnimationCurve();
    public AnimationCurve curveSpeedY = new AnimationCurve();
    void SetSpeedY(float speedY)
    {
        Vector2 vel = m_rigidBody2D.velocity;
        vel.y = speedY;
        curveSpeedY.AddKey(Time.realtimeSinceStartup, speedY);
        m_rigidBody2D.velocity = vel;
    }

    void Flip()
    {
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
    }

    void Die()
    {
        if (!m_isLive)
        {
            return;
        }

        m_rigidBody2D.velocity = new Vector2(0f, m_rigidBody2D.velocity.y);
        gameObject.layer = LayerMask.NameToLayer("CameraBoundary");
        m_isLive = false;
        m_animator.SetBool("Ground", true);
        m_animator.SetTrigger("Death");
    }
}
