using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl3 : MonoBehaviour
{
    public float moveSpeed = 8f;
    private Rigidbody2D m_rigidbody2D;
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;
    private float m_currentSpeed;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
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
}
