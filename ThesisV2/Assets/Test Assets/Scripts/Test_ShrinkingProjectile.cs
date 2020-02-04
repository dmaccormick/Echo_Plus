using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ShrinkingProjectile : MonoBehaviour
{
    //--- Public Variables ---//
    public float m_lifetime;



    //--- Private Variables ---//
    private Vector3 m_startScale;
    private float m_lifeSoFar;



    //--- Unity Methods ---//
    private void Start()
    {
        // Init the private variables
        m_startScale = transform.localScale;
        m_lifeSoFar = 0.0f;
    }

    private void Update()
    {
        // Update the life so far
        m_lifeSoFar += Time.deltaTime;

        // Calculate the lerp T value
        float lerpT = m_lifeSoFar / m_lifetime;

        // Slowly shrink to 0 on all axes
        this.transform.localScale = Vector3.Lerp(m_startScale, Vector3.zero, lerpT);

        // If the time has passed, destroy this object
        if (m_lifeSoFar >= m_lifetime)
            Destroy(this.gameObject);
    }
}
