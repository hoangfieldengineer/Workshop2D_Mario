using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterControl : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float moveSpeedFastMode = 12f;
    public bool m_moveFast = false;

    private float Speed
    {
        get { return m_moveFast ? moveSpeedFastMode : moveSpeed; }
    }

    public float accelerationSpeed = 2f;
    public float decelerationSpeed = 2f;

    private float m_currentSpeed;

    private Rigidbody2D m_rigidbody2D;
    
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_moveFast = Input.GetKey(KeyCode.A);
    }

    
    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float sign = Mathf.Sign(horizontalMove);

        if (Mathf.Abs(horizontalMove) > 0.001f)
        {
            float targetSpeed = sign * Speed;
            float maxDeltaChange = Time.deltaTime * (accelerationSpeed + (Mathf.Sign(m_currentSpeed) != sign ? decelerationSpeed : 0));

            m_currentSpeed = Mathf.MoveTowards(m_currentSpeed, targetSpeed, maxDeltaChange);
        }
        else
        {
            m_currentSpeed = Mathf.MoveTowards(m_currentSpeed, 0f, Time.deltaTime * decelerationSpeed);
        }

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

        m_animator.SetFloat("Speed",  Mathf.Abs(m_currentSpeed) / Speed);
        m_rigidbody2D.velocity = new Vector2(m_currentSpeed, m_rigidbody2D.velocity.y);
    }

    void Flip()
    {
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
    }
}
