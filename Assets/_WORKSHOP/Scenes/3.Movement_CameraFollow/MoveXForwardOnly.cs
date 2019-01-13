using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveXForwardOnly : MonoBehaviour
{
    public Transform target;

    private Transform m_trans;
    private Vector3 pos;
    void Awake()
    {
        m_trans = this.transform;
    }

    void Update()
    {
        if (m_trans.position.x < target.position.x)
        {
            pos = m_trans.position;
            pos.x = target.position.x;
            m_trans.position = pos;
        }
    }
}
