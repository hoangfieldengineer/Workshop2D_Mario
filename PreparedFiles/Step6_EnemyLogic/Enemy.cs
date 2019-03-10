using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool m_isTriggered = false;
    public Vector2 m_moveSpeed = new Vector2(-1f, 0f);

    [SerializeField] private Transform m_groundCheck;
    [SerializeField] private Transform m_frontCheck;
    [SerializeField] private LayerMask m_whatIsGround;    // A mask determining what is ground to the enemy

    private Animator m_animator;
    private Rigidbody2D m_rigidBody2D;
    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_collider;
    private Transform m_trans;
    private bool m_isGround = true;
    private RaycastHit2D[] hits = new RaycastHit2D[5];

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        m_trans = transform;
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTriggerEnter2D");
        if (!m_isTriggered)
        {
            if (collider.gameObject.CompareTag("CameraTrigger"))
            {
                m_isTriggered = true;
            }
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_isTriggered)
        {
            return;
        }

        m_isGround = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_groundCheck.position, k_GroundedRadius, m_whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_isGround = true;
        }
        m_animator.SetBool("Ground", m_isGround);

        if (m_isGround)
        {
            
            Vector3 pos = m_rigidBody2D.position + m_collider.offset;


            Debug.DrawRay(pos, m_frontCheck.position - pos, Color.red);
            int count = Physics2D.LinecastNonAlloc(pos, m_frontCheck.position, hits, m_whatIsGround);
            // Create an array of all the colliders in front of the enemy.
            if (count > 0)
            {
                // Check each of the colliders.
                for(int i = 0; i < count; ++i)
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
            if (m_moveSpeed.x > 0 && m_trans.localScale.x < 0)
            {
                // ... flip the player.
                //m_spriteRenderer.flipX = false;
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (m_moveSpeed.x < 0 && m_trans.localScale.x > 0)
            {
                // ... flip the player.
                //m_spriteRenderer.flipX = true;
                Flip();
            }
        }
    }

    public void Flip()
    {
        // Multiply the x component of localScale by -1.
        Vector3 enemyScale = m_trans.localScale;
        enemyScale.x *= -1;
        m_trans.localScale = enemyScale;
    }
}
