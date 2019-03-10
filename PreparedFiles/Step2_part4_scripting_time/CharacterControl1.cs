using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl1 : MonoBehaviour
{
    public float moveSpeed = 8f;
    private Rigidbody2D m_rigidbody2D;
    private float m_currentSpeed;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal"); // use arrow keys on keyboard will cause this variable changed, vary from -1 to 1
        Debug.Log("horizontalMove: " + horizontalMove);
        m_currentSpeed = horizontalMove * moveSpeed;

        m_rigidbody2D.velocity = new Vector2(m_currentSpeed, m_rigidbody2D.velocity.y);
    }
}
