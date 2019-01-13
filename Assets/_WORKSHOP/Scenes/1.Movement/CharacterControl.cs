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

    private float speedHorizontal;


    private bool m_isGround = true;
    private Rigidbody2D m_rigidbody2D;
    
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;
    private float m_previousSign = 1f;

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
            float maxDeltaTime = Time.deltaTime * (accelerationSpeed + (Mathf.Sign(speedHorizontal) != sign ? decelerationSpeed : 0));

            speedHorizontal = Mathf.MoveTowards(speedHorizontal, targetSpeed, maxDeltaTime);
        }
        else
        {
            speedHorizontal = Mathf.MoveTowards(speedHorizontal, 0f, Time.deltaTime * decelerationSpeed);
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

        m_animator.SetFloat("Speed",  Mathf.Abs(speedHorizontal) / Speed);
        m_rigidbody2D.velocity = new Vector2(speedHorizontal, m_rigidbody2D.velocity.y);
    }

    void Flip()
    {
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
    }
}
