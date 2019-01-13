using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public Vector2 m_moveSpeed = new Vector2(-1f, 0f);

    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private LayerMask m_WhatIsGround;    // A mask determining what is ground to the enemy

    private Animator m_animator;
    private Rigidbody2D m_rigidBody2D;
    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_collider;
    private bool m_isGround = true;

    private Vector2 m_frontCheck;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
    }

    private RaycastHit2D[] hits = new RaycastHit2D[5];
    // Update is called once per frame
    void FixedUpdate()
    {
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

        if (m_isGround)
        {
            Vector2 direction = new Vector2(m_spriteRenderer.flipX ? -m_collider.size.x / 2f : m_collider.size.x / 2f, 0f);
            direction.x += Mathf.Sign(direction.x) * (m_collider.edgeRadius + 0.1f);
            Vector2 pos = m_rigidBody2D.position + m_collider.offset;
            m_frontCheck = pos + direction;

            Debug.DrawRay(pos, direction, Color.red);
            int count = Physics2D.LinecastNonAlloc(pos, m_frontCheck, hits, m_WhatIsGround);
            // Create an array of all the colliders in front of the enemy.
            if (count > 0)
            {
                // Check each of the colliders.
                for (int i = 0; i < count; ++i)
                {
                    RaycastHit2D c = hits[i];
                    // If any of the colliders is an Obstacle...
                    if (!c.collider.gameObject.CompareTag("Player") && c.collider.gameObject != gameObject)
                    {
                        Debug.Log("Flip");
                        // ... Flip the enemy and stop checking the other colliders.
                        m_moveSpeed.x *= -1f;
                        break;
                    }
                }
            }

            m_rigidBody2D.velocity = m_moveSpeed;

            // If the input is moving the player right and the player is facing left...
            if (m_moveSpeed.x > 0 && m_spriteRenderer.flipX)
            {
                // ... flip the player.
                m_spriteRenderer.flipX = false;
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (m_moveSpeed.x < 0 && !m_spriteRenderer.flipX)
            {
                // ... flip the player.
                m_spriteRenderer.flipX = true;
            }
        }
    }
}
