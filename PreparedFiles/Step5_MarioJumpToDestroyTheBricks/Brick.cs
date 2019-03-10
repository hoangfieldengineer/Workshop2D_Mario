using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public ParticleSystem m_particle;
    public bool m_breakable = true;

    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_breakable && collision.gameObject.CompareTag("Player") /*&& collision.contactCount > 0*/)
        {
            ContactPoint2D cp = collision.contacts[0];
            if (cp.normal == Vector2.up)
            {
                collision.otherCollider.enabled = false;
                Explode();
            }
        }
    }

    void Explode()
    {
        Debug.Log("Brick explode");
        if (m_particle)
        {
            ParticleSystem part = Instantiate(m_particle) ; //, transform.position, Quaternion.identity);
            part.transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}
